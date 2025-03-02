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

namespace Jaller.Core.FileManagement;

internal sealed class JallerFileManager : IJallerFileManager
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    private readonly JallerDatabase db;

    // ---------------- Constructor ----------------

    public JallerFileManager( IJallerCore core, JallerDatabase db )
    {
        this.core = core;
        this.db = db;
    }

    // ---------------- Methods ----------------

    public int GetFileCount()
    {
        return db.Files.Count();
    }

    public JallerFile? TryGetFile( string cid )
    {
        Cid realCid = Cid.Parse( cid );

        IpfsFile? file = this.db.Files.FindById( realCid.Version1Cid );
        if( file is null )
        {
            return null;
        }

        return new JallerFile
        {
            CidV1 = file.Cid,
            Description = file.Description,
            DownloadablePolicy = file.DownloadablePolicy,
            MetadataPrivacy = file.MetadataPrivacy,
            MimeType = file.MimeType,
            Name = file.FileName,
            ParentFolder = file.ParentFolder,
            Tags = file.Tags
        };
    }


    public void ConfigureFile( JallerFile file )
    {
        JallerDirectory? newParentDirectory = null;
        if( file.ParentFolder is not null )
        {
            newParentDirectory = this.db.Directories.FindById( file.ParentFolder );
            if( newParentDirectory is null )
            {
                throw new FolderNotFoundException( $"Parent folder file needs to go to does not exist." );
            }
        }

        this.db.BeginTransaction();
        try
        {
            bool existing;
            int? oldFolderId;
            IpfsFile? dbFile = this.db.Files.FindById( file.CidV1 );
            if( dbFile is null )
            {
                existing = false;
                oldFolderId = null;

                dbFile = new IpfsFile
                {
                    Cid = file.CidV1
                };
            }
            else
            {
                existing = true;
                oldFolderId = dbFile.ParentFolder;
            }

            dbFile = dbFile with
            {
                Description = file.Description,
                DownloadablePolicy = file.DownloadablePolicy,
                MetadataPrivacy = file.MetadataPrivacy,
                MimeType = file.MimeType,
                FileName = file.Name,
                ParentFolder = file.ParentFolder,
                Tags = file.Tags
            };

            JallerDirectory? oldParentDirectory = null;

            if( ( oldFolderId == dbFile.ParentFolder ) && oldFolderId.HasValue )
            {
                if( this.db.Directories.FindById( dbFile.ParentFolder ) is null )
                {
                    // If the parent folder no longer does exists,
                    // consider it orphaned and move the file to the root.
                    dbFile = dbFile with
                    {
                        ParentFolder = null
                    };
                }
            }
            else if( oldFolderId != dbFile.ParentFolder )
            {
                if( oldFolderId is not null )
                {
                    oldParentDirectory = this.db.Directories.FindById( oldFolderId.Value );

                    // Leave null if the old directory does not exist,
                    // we'll have to garbage collect the old directory eventually.
                }
            }

            // First add the file.
            if( existing )
            {
                if( this.db.Files.Update( dbFile ) == false )
                {
                    throw new DatabaseException( "Failed to update file." );
                }
            }
            else
            {
                this.db.Files.Insert( dbFile );
            }

            // Update the directory to include the file in its child files.
            if( newParentDirectory is not null )
            {
                List<string>? files = newParentDirectory.Files;
                if( files is null )
                {
                    files = new List<string>();
                }
                files.Add( dbFile.Cid );

                newParentDirectory = newParentDirectory with
                {
                    Files = files
                };

                if( this.db.Directories.Update( newParentDirectory ) == false )
                {
                    throw new DatabaseException( "Unable to add file to parent directory." );
                }
            }

            // Remove from old parent directory.
            if( oldParentDirectory is not null )
            {
                List<string>? files = oldParentDirectory.Files;
                if( files is not null )
                {
                    files.Remove( dbFile.Cid );
                    if( files.Any() == false )
                    {
                        files = null;
                    }

                    oldParentDirectory = oldParentDirectory with
                    {
                        Files = files
                    };

                    if( this.db.Directories.Update( oldParentDirectory ) == false )
                    {
                        throw new DatabaseException( "Unable to remove file from old parent directory." );
                    }
                }
                else
                {
                    // Somehow null?  Shouldn't be the case if the file was here,
                    // but I guess we'll let it go.
                }
            }
        }
        catch( Exception )
        {
            this.db.Rollback();
            throw;
        }
        this.db.Commit();
    }

    public void DeleteFile( string fileCid )
    {
        Cid realCid = Cid.Parse( fileCid );

        IpfsFile? file = this.db.Files.FindById( realCid.Version1Cid );
        if( file is null )
        {
            // File is already deleted, no need to continue.
            return;
        }

        this.db.BeginTransaction();
        try
        {
            JallerDirectory? parentFolder = null;
            if( file.ParentFolder is not null )
            {
                parentFolder = this.db.Directories.FindById( file.ParentFolder );

                if(
                    ( parentFolder.Files is null ) ||
                    ( parentFolder.Files.Contains( file.Cid ) == false )
                )
                {
                    // If the parent folder doesn't even contain the file,
                    // no need to modify the folder at all, set it to null
                    // and we won't touch it.
                    parentFolder = null;
                }
            }

            if( parentFolder is not null )
            {
                List<string>? files = parentFolder.Files;
                if( files is not null )
                {
                    files.Remove( file.Cid );
                    if( files.Any() == false )
                    {
                        files = null;
                    }
                }
                parentFolder = parentFolder with
                {
                    Files = files
                };

                if( this.db.Directories.Update( parentFolder ) == false )
                {
                    throw new DatabaseException(
                        $"Unable to delete file from parent folder."
                    );
                }
            }

            if( this.db.Files.Delete( file.Cid ) == false )
            {
                throw new DatabaseException( $"Unable to delete file." );
            }
        }
        catch( Exception )
        {
            this.db.Rollback();
            throw;
        }
        this.db.Commit();
    }
}
