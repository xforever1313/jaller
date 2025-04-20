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

using Jaller.Contracts.About;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Server.Extensions;

public static class PageModelExtensions
{
    // ---------------- Methods ----------------

    public static IJallerCore Core( this PageModel model )
    {
        IJallerCore? core = model.HttpContext.RequestServices.GetService<IJallerCore>();
        if( core is null )
        {
            throw new InvalidOperationException( $"Could not get {nameof( IJallerCore )}" );
        }

        return core;
    }

    public static JallerVersionInfo JallerVersionInfo( this PageModel model ) =>
        Resources.GetVersionInfo();

    public static DownloadPolicy UserDownloadPolicy( this PageModel model ) => 
        model.User.GetUserDownloadPolicy();

    public static MetadataPolicy UserMetadataPolicy( this PageModel model ) => 
        model.User.GetUserMetadataPolicy();

    /// <summary>
    /// Returns true if the logged in user can edit metadata.
    /// </summary>
    public static bool AllowMetadataEdit( this PageModel model )
        => model.User.CanUserEditMetaData();

    /// <summary>
    /// Returns true if the logged in user can upload files.
    /// The user must have permission, and Jaller must be connected to Kubo.
    /// </summary>
    public static bool AllowFileUpload( this PageModel model ) =>
        model.User.CanUserUploadFiles( model.Core() );

    public static bool AllowAdminAccess( this PageModel model ) =>
        model.User.CanUserAccessAdminPanel( model.Core(), model.Request );

    public static bool AllowPublicRegistration( this PageModel model ) =>
        model.Core().Config.Users.AllowPublicRegistration;
}