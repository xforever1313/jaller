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
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core
{
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
            return this.db.Directories.Count();
        }

        public int ConfigureFolder( JallerFolder newFolderConfig )
        {
            JallerDirectory? dbDirectory = this.db.Directories.FindById( newFolderConfig.Id );

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

            if( oldParentId != newFolderConfig.ParentFolder )
            {
                if( oldParentId is not null )
                {
                    oldParentDirectory = this.db.Directories.FindById( oldParentId.Value );
                    if( oldParentDirectory is null )
                    {
                        throw new DirectoryNotFoundException(
                            $"Can not the original parent directory with an ID of {oldParentId}."
                        );
                    }
                }

                if( newFolderConfig.ParentFolder is not null )
                {
                    newParentDirectory = this.db.Directories.FindById( newFolderConfig.ParentFolder );
                    if( newParentDirectory is null )
                    {
                        throw new DirectoryNotFoundException(
                            $"Can not the new parent directory with an ID of {oldParentId}."
                        );
                    }
                }
            }

            this.db.BeginTransaction();
            try
            {
                if( existing )
                {
                    if( this.db.Directories.Update( dbDirectory ) == false )
                    {
                        throw new DatabaseException( "Failed to update directory." );
                    }
                }
                else
                {
                    if( this.db.Directories.Insert( dbDirectory ) == false )
                    {
                        throw new DatabaseException( "Failed to create directory." );
                    }
                }

                if( ( oldParentDirectory is not null ) && ( oldParentDirectory.ChildrenFolders is not null ) )
                {
                    oldParentDirectory.ChildrenFolders.Remove( newFolderConfig.Id );
                    this.db.Directories.Update( oldParentDirectory );
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
                    this.db.Directories.Update( newParentDirectory );
                }
            }
            catch( Exception )
            {
                this.db.Rollback();
                throw;
            }

            this.db.Commit();

            return dbDirectory.Id;
        }

        public void DeleteFolder( int folderId )
        {
            JallerDirectory? directory = this.db.Directories.FindById( folderId );
            if( directory is null )
            {
                // No-op, the directory already doesn't exist.
                return;
            }

            try
            {
                this.db.BeginTransaction();
                DeleteFolderInternal( folderId );
            }
            catch( Exception e1 )
            {
                try
                {
                    this.db.Rollback();
                }
                catch( Exception e2 )
                {
                    throw new AggregateException( e1, e2 );
                }
                throw;
            }

            this.db.Commit();
        }

        private void DeleteFolderInternal( int folderId )
        {
            JallerDirectory? directory = this.db.Directories.FindById( folderId );
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
                List<IpfsFile>? files = directory.Files;
                if( files is not null )
                {
                    foreach( IpfsFile file in files )
                    {
                        this.core.Files.DeleteFile( file.Cid );
                    }
                }
            }

            // Next, delete the folder from the parent's folder list,
            // if there is a parent folder that is.
            if( directory.ParentFolder is not null )
            {
                JallerDirectory? parentDirectory = this.db.Directories.FindById( directory.ParentFolder );
                if( parentDirectory is not null )
                {
                    List<int>? parentsChildFolders = parentDirectory.ChildrenFolders;
                    if( parentsChildFolders is not null )
                    {
                        parentsChildFolders.Remove( directory.Id );
                    }

                    if( this.db.Directories.Update( parentDirectory ) == false )
                    {
                        throw new DatabaseException( "Failed to remove directory marked for deletion from parent folder." );
                    }
                }
            }

            // Now, we can finally delete this folder.
            bool deleted = this.db.Directories.Delete( folderId );
            if( deleted == false )
            {
                throw new DatabaseException( "Failed to delete directory marked for deletion." );
            }
        }

        public FolderContents GetRootFolder( FileMetadataPolicy visibility )
        {
            throw new NotImplementedException();
        }

        public JallerFolder? TryGetFolder( int id )
        {
            JallerDirectory? dir = this.db.Directories.FindById( id );
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
            JallerDirectory? directory = this.db.Directories.FindById( folderId );
            if ( directory is null )
            {
                return null;
            }

            List<JallerFolder>? folders = null;
            List<int>? childFolders = directory.ChildrenFolders;
            if( childFolders is not null )
            {
                folders = new List<JallerFolder>();
                foreach( int childId in childFolders )
                {
                    JallerDirectory? childDirectory = this.db.Directories.FindById( childId );
                    if( childDirectory is null )
                    {
                        throw new DirectoryNotFoundException(
                            $"Can not find child with ID of {childId} in parent folder {folderId}."
                        );
                    }

                    folders.Add(
                        new JallerFolder
                        {
                            Id = childDirectory.Id,
                            Name = childDirectory.Name,
                            ParentFolder = directory.Id,
                        }
                    );
                }
            }

            return new FolderContents
            {
                ChildFolders = folders,
                Files = null
            };
        }
    }
}
