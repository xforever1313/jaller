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
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Folder
{
    public sealed class AddModel : BasePageModel
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

        public string? ErrorMessage { get; private set; }

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
                    this.ErrorMessage = $"Can not find parent folder with ID {parentFolderId}.";
                    this.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Page();
                }
            }

            return Page();
        }
    }
}
