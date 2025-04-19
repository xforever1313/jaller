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
using Jaller.Server.Extensions;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Server.Models
{
    public class BasePageModel : PageModel
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public BasePageModel( IJallerCore core )
        {
            this.core = core;

            this.JallerVersionInfo = Resources.GetVersionInfo();
        }

        // ---------------- Properties ----------------

        public JallerVersionInfo JallerVersionInfo { get; }

        public DownloadPolicy UserDownloadPolicy => this.User.GetUserDownloadPolicy();

        public MetadataPolicy UserMetadataPolicy => this.User.GetUserMetadataPolicy();

        /// <summary>
        /// Returns true if the logged in user can edit metadata.
        /// </summary>
        public bool AllowMetadataEdit => this.User.CanUserEditMetaData();

        /// <summary>
        /// Returns true if the logged in user can upload files.
        /// The user must have permission, and Jaller must be connected to Kubo.
        /// </summary>
        public bool AllowFileUpload => this.User.CanUserUploadFiles( this.core );

        public bool AllowAdminAccess => this.User.CanUserAccessAdminPanel( this.core, this.Request );

        public bool AllowPublicRegistration => this.core.Config.Users.AllowPublicRegistration;
    }
}
