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
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Folder;

public sealed class DeleteModel : BasePageModel, IAlert
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public DeleteModel( IJallerCore core ) :
        base( core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public JallerFolder? JallerFolder { get; private set; }

    [BindProperty]
    public int? FolderToDelete { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFolderInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFolderWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFolderErrorMessage" )]
    public string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( int? id )
    {
        if( this.core.Config.Web.IsAdminRequstAllowed( this.Request ) == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        // If no ID is specified, assume root directory.
        id = id ?? 0;

        if( id == 0 )
        {
            this.ErrorMessage = "Can not delete root folder.";
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Page();
        }

        this.JallerFolder = await Task.Run( () => this.core.Folders.TryGetFolder( id.Value ) );

        if( this.JallerFolder is null )
        {
            this.ErrorMessage = "Can not find folder at the specified ID.";
            this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if( this.core.Config.Web.IsAdminRequstAllowed( this.Request ) == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        int folderToDelete = this.FolderToDelete ?? 0;
        if( folderToDelete == 0 )
        {
            this.ErrorMessage = "Root folder can not be deleted.";
            return RedirectToPage();
        }

        try
        {
            await Task.Run( () => this.core.Folders.DeleteFolder( folderToDelete ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        this.InfoMessage = null;
        this.WarningMessage = null;
        this.ErrorMessage = null;

        return RedirectToPage( "FolderDeleteConfirmation", new { id = folderToDelete } );
    }
}
