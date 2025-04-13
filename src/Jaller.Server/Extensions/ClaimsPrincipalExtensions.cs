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
using Jaller.Server.Extensions;
using Jaller.Standard;
using Jaller.Standard.FileManagement;

namespace Jaller.Server;

public static class ClaimsPrincipalExtensions
{
    // ---------------- Methods ----------------

    public static bool IsLoggedIn( this ClaimsPrincipal? user )
    {
        if( user is null )
        {
            return false;
        }

        return user.Identity?.IsAuthenticated == true;
    }

    public static UserRolesModel ToRolesModel( this ClaimsPrincipal? user, IJallerCore core, HttpRequest request )
    {
        if( user is null )
        {
            return new UserRolesModel();
        }

        return new UserRolesModel
        {
            IsLoggedIn = user.IsLoggedIn(),
            IsApprovedUser = user.IsUserApproved(),
            IsEditor = user.CanUserEditMetaData(),
            IsUploader = user.CanUserUploadFiles( core ),
            IsAdmin = user.CanUserAccessAdminPanel( core, request )
        };
    }

    public static DownloadPolicy GetUserDownloadPolicy( this ClaimsPrincipal? user )
    {
        if( user is null )
        {
            return DownloadPolicy.Public;
        }
        else if( user.IsInRole( Roles.User.GetRoleName() ) )
        {
            // User must be logged in to download private files.
            return DownloadPolicy.Private;
        }
        else
        {
            return DownloadPolicy.Public;
        }
    }

    public static MetadataPolicy GetUserMetadataPolicy( this ClaimsPrincipal? user )
    {
        if( user is null )
        {
            return MetadataPolicy.Public;
        }
        else if( user.IsInRole( Roles.User.GetRoleName() ) )
        {
            // User must be logged in to see private files.
            return MetadataPolicy.Private;
        }
        else
        {
            return MetadataPolicy.Public;
        }
    }

    public static bool IsUserApproved( this ClaimsPrincipal? user )
    {
        if( user is null )
        {
            return false;
        }

        return user.IsInRole( Roles.User.GetRoleName() );
    }

    public static bool CanUserEditMetaData( this ClaimsPrincipal? user )
    {
        if( user is null )
        {
            return false;
        }

        return user.IsInRole( Roles.Editor.GetRoleName() );
    }

    public static bool CanUserUploadFiles( this ClaimsPrincipal? user, IJallerCore core )
    {
        if( user is null )
        {
            return false;
        }
        else if( core.Config.Ipfs.KuboUrl is null )
        {
            return false;
        }

        return user.IsInRole( Roles.Uploader.GetRoleName() );
    }

    public static bool CanUserAccessAdminPanel( this ClaimsPrincipal? user, IJallerCore core, HttpRequest request )
    {
        if( user is null )
        {
            return false;
        }
        else if( core.Config.Web.IsAdminRequstAllowed( request ) == false )
        {
            return false;
        }
        
        return user.IsInRole( Roles.Admin.GetRoleName() );
    }
}