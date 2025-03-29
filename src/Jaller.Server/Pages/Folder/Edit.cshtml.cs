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

public sealed class EditModel : BasePageModel, IAlert
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public EditModel( IJallerCore core ) :
        base( core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public JallerFolder? JallerFolder { get; private set; }

    [BindProperty]
    public JallerFolder? UploadedFolder { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "EditInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "EditWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "EditErrorMessage" )]
    public string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( int? id )
    {
        // If no ID is specified, assume root directory.
        id = id ?? 0;

        if( id == 0 )
        {
            this.ErrorMessage = "Can not edit root folder.";
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

        if( this.UploadedFolder is null )
        {
            this.ErrorMessage = "File was somehow null.";
            return RedirectToPage();
        }
        else if( this.UploadedFolder.Id == 0 )
        {
            this.ErrorMessage = "Can not edit the root folder.";
            return RedirectToPage();
        }

        int folderId;
        try
        {
            folderId = await Task.Run( () => this.core.Folders.ConfigureFolder( this.UploadedFolder ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        this.InfoMessage = $"Folder Updated!  Its ID is {folderId}.";
        this.WarningMessage = null;
        this.ErrorMessage = null;
        return RedirectToPage( new { id = folderId } );
    }
}
