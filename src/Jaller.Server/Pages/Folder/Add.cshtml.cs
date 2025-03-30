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

using System.Net;
using Jaller.Server.Extensions;
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Folder
{
    public sealed class AddModel : BasePageModel, IAlert, IJallerPermissions
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public AddModel( IJallerCore core ) :
            base( core )
        {
            this.core = core;
        }

        // ---------------- Properties ----------------

        public JallerFolder? ParentFolder { get; private set; }

        // -------- POST Properties --------

        [BindProperty]
        public string? NewFolderName { get; set; }

        [BindProperty]
        public int? ParentFolderId { get; set; }

        [BindProperty]
        public MetadataPolicy? MetadataPrivacy { get; set; }

        [BindProperty]
        public DownloadPolicy? DownloadablePolicy { get; set; }

        // -------- Messages --------

        /// <summary>
        /// Error message that appears during a get request.
        /// Null for no error.
        /// </summary>
        public string? GetRequestErrorMessage { get; private set; }

        /// <inheritdoc/>
        [TempData( Key = "AddFolderInfoMessage" )]
        public string? InfoMessage { get; set; }

        /// <inheritdoc/>
        [TempData( Key = "AddFolderWarningMessage" )]
        public string? WarningMessage { get; set; }

        /// <inheritdoc/>
        [TempData( Key = "AddFolderErrorMessage" )]
        public string? ErrorMessage { get; set; }

        // ---------------- Methods ----------------

        public async Task<IActionResult> OnGetAsync( int? parentFolderId )
        {
            // If no parent folder is specified, assume the root directory.
            // Also, if the id is 0, assume root directory.
            if( parentFolderId is not null && ( parentFolderId.Value != 0 ) )
            {
                this.ParentFolder = await Task.Run( () => this.core.Folders.TryGetFolder( parentFolderId.Value ) );
                if( this.ParentFolder is null )
                {
                    this.GetRequestErrorMessage = $"Can not find parent folder with ID {parentFolderId}.";
                    this.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Page();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.InfoMessage = null;
            this.WarningMessage = null;
            this.ErrorMessage = null;

            if( this.core.Config.Web.IsAdminRequstAllowed( this.Request ) == false )
            {
                return StatusCode( (int)HttpStatusCode.Forbidden );
            }

            if( this.ParentFolderId == 0 )
            {
                this.ParentFolderId = null;
            }

            var newFolder = new JallerFolder
            {
                Name = this.NewFolderName ?? "Untitled Folder",
                DownloadablePolicy = this.DownloadablePolicy ?? JallerFolder.DefaultDownloadablePolicy,
                MetadataPrivacy = this.MetadataPrivacy ?? JallerFolder.DefaultMetadataPrivacy,
                ParentFolder = this.ParentFolderId
            };

            int newId;
            try
            {
                newId = await Task.Run( () => this.core.Folders.ConfigureFolder( newFolder ) );
            }
            catch( Exception e )
            {
                this.ErrorMessage = e.Message;
                return RedirectToPage();
            }

            this.InfoMessage = $"Folder Added!  Its ID is {newId}.";
            return RedirectToPage( new { parentFolderId = this.ParentFolderId } );
        }
    }
}
