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
using Jaller.Core.FileManagement;
using Jaller.Standard.FileManagement;
using SethCS.Extensions;

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

    public static RenderableMimeType IsRenderable( this JallerFile file )
    {
        string mimeType = file.GetMimeType();
        if( mimeType.EqualsIgnoreCase( "application/pdf" ) )
        {
            return RenderableMimeType.Pdf;
        }
        else if( mimeType.StartsWith( "audio/", StringComparison.OrdinalIgnoreCase ) )
        {
            return RenderableMimeType.Audio;
        }
        else if( mimeType.StartsWith( "video/", StringComparison.OrdinalIgnoreCase ) )
        {
            return RenderableMimeType.Video;
        }
        else if( mimeType.StartsWith( "image/", StringComparison.OrdinalIgnoreCase ) )
        {
            return RenderableMimeType.Image;
        }
        else if( mimeType.EqualsIgnoreCase( "text/plain" ) )
        {
            return RenderableMimeType.PlainText;
        }
        else
        {
            return RenderableMimeType.NotRenderable;
        }
    }
}
