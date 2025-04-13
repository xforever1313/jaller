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
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Folder;

public class FolderDeleteConfirmationModel : BasePageModel
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public FolderDeleteConfirmationModel( IJallerCore core ) :
        base( core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    public int? DeletedFolderId { get; private set; }

    // ---------------- Properties ----------------

    public IActionResult OnGet( int? id )
    {
        if( this.AllowMetadataEdit == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        this.DeletedFolderId = id;

        return Page();
    }
}
