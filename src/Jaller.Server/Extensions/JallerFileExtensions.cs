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

using System.Security.Claims;
using Jaller.Contracts.FileManagement;
using Jaller.Standard.FileManagement;

namespace Jaller.Server.Extensions;

public static class JallerFileExtensions
{
    // ---------------- Methods ----------------

    public static JallerFileInfo ToFileInfo( this JallerFile file )
    {
        return new JallerFileInfo
        {
            CidV1 = file.CidV1,
            Description = file.Description,
            MimeType = file.MimeType,
            Name = file.Name,
            ParentFolderId = file.ParentFolder,
            Tags = file.Tags?.ToArray() ?? null
        };
    }

    public static JallerFileTreeLeafInfo ToTreeLeafFileInfo( this JallerFile file )
    {
        return new JallerFileTreeLeafInfo
        {
            CidV1 = file.CidV1,
            Name = file.Name,
            ParentFolderId = file.ParentFolder,
        };
    }

    public static bool IsDownloadable( this JallerFile? file, ClaimsPrincipal user )
    {
        if( file is null )
        {
            return false;
        }
        if( file.DownloadablePolicy == DownloadPolicy.Public )
        {
            return true;
        }
        else if( file.DownloadablePolicy == DownloadPolicy.NoDownload )
        {
            return false;
        }
        else if( file.DownloadablePolicy == DownloadPolicy.Private )
        {
            if( user.IsUserApproved() )
            {
                return true;
            }
        }

        // All else fails, assume we don't want the file downloaded.
        return false;
    }
}
