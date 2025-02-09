//
// Jaller - An advanced IPFS Gateway
// Copyright (C) 2025 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Jaller.Core.Database;
using Jaller.Core.Exceptions;
using Jaller.Core.FileManagement;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core.FolderManagement;

internal sealed class FolderManager : IFolderManager
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    private readonly JallerDatabase db;

    // ---------------- Constructor ----------------

    public FolderManager( IJallerCore core, JallerDatabase db )
    {
        this.core = core;
        this.db = db;
    }

    // ---------------- Methods ----------------

    public int GetFolderCount()
    {
        return db.Directories.Count();
    }

    public int ConfigureFolder( JallerFolder newFolderConfig )
    {
        db.BeginTransaction();
        try
        {
            JallerDirectory? dbDirectory = db.Directories.FindById( newFolderConfig.Id );

            bool existing;
            int? oldParentId;
            if( dbDirectory is null ) // Directory does not exist create a new one.
            {
                existing = false;
                oldParentId = null;

                // This is a new folder
                dbDirectory = new JallerDirectory
                {
                    // No children folders yet, this is a new directory.
                    ChildrenFolders = null,

                    // No files yet, this is a new directory.
                    Files = null,

                    Name = newFolderConfig.Name,
                    ParentFolder = newFolderConfig.ParentFolder
                };
            }
            else // Directory exists, and we want to modify it.
            {
                existing = true;
                oldParentId = dbDirectory.ParentFolder;

                dbDirectory = dbDirectory with
                {
                    Name = newFolderConfig.Name,
                    ParentFolder = newFolderConfig.ParentFolder
                };
            }

            JallerDirectory? oldParentDirectory = null;
            JallerDirectory? newParentDirectory = null;

            if( oldParentId == newFolderConfig.ParentFolder )
            {
                if( oldParentId is not null )
                {
                    if( db.Directories.FindById( oldParentId.Value ) is null )
                    {
                        // If a parent folder does not exist, consider it orphaned
                        // and move the folder to the root.
                        dbDirectory = dbDirectory with
                        {
                            ParentFolder = null
                        };
                    }
                }
            }
            else if( oldParentId != newFolderConfig.ParentFolder )
            {
                if( oldParentId is not null )
                {
                    oldParentDirectory = db.Directories.FindById( oldParentId.Value );

                    // Leave it null if it does not exist.
                    // We'll have to garbage collect the old directory.
                }

                if( newFolderConfig.ParentFolder is not null )
                {
                    newParentDirectory = db.Directories.FindById( newFolderConfig.ParentFolder );
                    if( newParentDirectory is null )
                    {
                        throw new FolderNotFoundException(
                            $"Can not the new parent directory with an ID of {oldParentId}."
                        );
                    }
                }
            }

            if( existing )
            {
                if( db.Directories.Update( dbDirectory ) == false )
                {
                    throw new DatabaseException( "Failed to update directory." );
                }
            }
            else
            {
                if( db.Directories.Insert( dbDirectory ) == false )
                {
                    throw new DatabaseException( "Failed to create directory." );
                }
            }

            if( oldParentDirectory is not null && oldParentDirectory.ChildrenFolders is not null )
            {
                oldParentDirectory.ChildrenFolders.Remove( newFolderConfig.Id );
                db.Directories.Update( oldParentDirectory );
            }

            if( newParentDirectory is not null )
            {
                if( newParentDirectory.ChildrenFolders is null )
                {
                    newParentDirectory = newParentDirectory with
                    {
                        ChildrenFolders = new List<int>()
                    };
                }
                newParentDirectory.ChildrenFolders.Add( dbDirectory.Id );
                db.Directories.Update( newParentDirectory );
            }

            db.Commit();

            return dbDirectory.Id;
        }
        catch( Exception )
        {
            db.Rollback();
            throw;
        }
    }

    public void DeleteFolder( int folderId )
    {
        JallerDirectory? directory = db.Directories.FindById( folderId );
        if( directory is null )
        {
            // No-op, the directory already doesn't exist.
            return;
        }

        try
        {
            db.BeginTransaction();
            DeleteFolderInternal( folderId );
        }
        catch( Exception e1 )
        {
            try
            {
                db.Rollback();
            }
            catch( Exception e2 )
            {
                throw new AggregateException( e1, e2 );
            }
            throw;
        }

        db.Commit();
    }

    private void DeleteFolderInternal( int folderId )
    {
        JallerDirectory? directory = db.Directories.FindById( folderId );
        if( directory is null )
        {
            // No-op, the directory already doesn't exist.
            return;
        }

        // First, need to delete all of the children directories.
        {
            List<int>? childFolders = directory.ChildrenFolders;
            if( childFolders is not null )
            {
                foreach( int childFolder in childFolders )
                {
                    DeleteFolderInternal( childFolder );
                }
            }
        }

        // Second, delete all files in this folder.
        {
            List<string>? files = directory.Files;
            if( files is not null )
            {
                foreach( string file in files )
                {
                    core.Files.DeleteFile( file );
                }
            }
        }

        // Next, delete the folder from the parent's folder list,
        // if there is a parent folder that is.
        if( directory.ParentFolder is not null )
        {
            JallerDirectory? parentDirectory = db.Directories.FindById( directory.ParentFolder );
            if( parentDirectory is not null )
            {
                List<int>? parentsChildFolders = parentDirectory.ChildrenFolders;
                if( parentsChildFolders is not null )
                {
                    parentsChildFolders.Remove( directory.Id );
                }

                if( db.Directories.Update( parentDirectory ) == false )
                {
                    throw new DatabaseException( "Failed to remove directory marked for deletion from parent folder." );
                }
            }
        }

        // Now, we can finally delete this folder.
        bool deleted = db.Directories.Delete( folderId );
        if( deleted == false )
        {
            throw new DatabaseException( "Failed to delete directory marked for deletion." );
        }
    }

    public FolderContents GetRootFolder( FileMetadataPolicy visibility )
    {
        IEnumerable<JallerDirectory> rootDirs = this.db.Directories.Find( d => d.ParentFolder.HasValue == false );
        IEnumerable<IpfsFile> rootIpfsFiles = this.db.Files.Find( d => d.ParentFolder.HasValue == false );

        List<JallerFolder>? rootFolders = null;
        List<JallerFile>? rootFiles = null;

        if( rootDirs.Any() )
        {
            rootFolders = rootDirs.Select( d => d.ToPublicModel() ).ToList();
        }

        if( rootIpfsFiles.Any() )
        {
            rootFiles = rootIpfsFiles.Select( f => f.ToPublicModel() ).ToList();
        }

        return new FolderContents
        {
            ChildFolders = rootFolders,
            Files = rootFiles
        };
    }

    public JallerFolder? TryGetFolder( int id )
    {
        JallerDirectory? dir = db.Directories.FindById( id );
        if( dir is null )
        {
            return null;
        }

        return new JallerFolder
        {
            Id = dir.Id,
            Name = dir.Name,
            ParentFolder = dir.ParentFolder
        };
    }

    public FolderContents? TryGetFolderContents( int folderId, FileMetadataPolicy visibility )
    {
        JallerDirectory? directory = db.Directories.FindById( folderId );
        if( directory is null )
        {
            return null;
        }

        List<JallerFolder>? folders = null;
        {
            bool garbageCollected = false;
            List<int>? childDirectories = directory.ChildrenFolders;
            if( childDirectories is not null )
            {
                folders = new List<JallerFolder>();
                foreach( int childId in childDirectories.ToList() )
                {
                    JallerDirectory? childDirectory = db.Directories.FindById( childId );
                    if( childDirectory is null )
                    {
                        // If we can't find the child directory, it probably got deleted
                        // or the app crashed.  Garbage collect it since it no longer exists.
                        garbageCollected = true;
                        if( childDirectories.Remove( childId ) == false )
                        {
                            throw new DatabaseException( $"Failed to garbage collect orphaned child folder with ID {childId}." );
                        }
                    }
                    else
                    {
                        folders.Add( childDirectory.ToPublicModel() );
                    }
                }

                // If there are no more child folders due to being
                // garbage collected, null it out to save on space.
                if( childDirectories.Any() == false )
                {
                    childDirectories = null;
                }

                if( folders.Any() == false )
                {
                    folders = null;
                }
            }

            if( garbageCollected )
            {
                directory = directory with
                {
                    ChildrenFolders = childDirectories
                };
                if( db.Directories.Update( directory ) == false )
                {
                    throw new DatabaseException(
                        "Unable to garbage collect an orphaned directory from parent directory."
                    );
                }
            }
        }

        List<JallerFile>? files = null;
        {
            bool garbageCollected = false;
            List<string>? fileCids = directory.Files;
            if( fileCids is not null )
            {
                files = new List<JallerFile>();
                foreach( string cid in fileCids.ToList() )
                {
                    IpfsFile? file = this.db.Files.FindById( cid );
                    if( file is null )
                    {
                        fileCids.Remove( cid );
                        garbageCollected = true;
                    }
                    else
                    {
                        files.Add( file.ToPublicModel() );
                    }
                }
            }

            if( garbageCollected )
            {
                directory = directory with
                {
                    Files = fileCids
                };

                if( db.Directories.Update( directory ) == false )
                {
                    throw new DatabaseException(
                        "Unable to garbage collect an orphaned file from directory."
                    );
                }
            }
        }

        return new FolderContents
        {
            ChildFolders = folders,
            Files = files
        };
    }
}
