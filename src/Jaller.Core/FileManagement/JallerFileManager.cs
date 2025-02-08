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
        IpfsFile? file = this.db.Files.FindById( cid );
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
            ParentFolder = file.ParentFolder
        };
    }


    public void ConfigureFile( JallerFile file )
    {
        throw new NotImplementedException();
    }

    public void DeleteFile( string fileCid )
    {

        IpfsFile? file = this.db.Files.FindById( fileCid );
        if( file is null )
        {
            // File is already deleted, no need to continue.
            return;
        }

        db.BeginTransaction();
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
            db.Rollback();
            throw;
        }
        db.Commit();
    }
}
