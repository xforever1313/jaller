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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Server.Pages.File;

public sealed class DeleteModel : PageModel, IAlert
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public DeleteModel( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public JallerFile? JallerFile { get; private set; }

    [BindProperty]
    public string? FileToDelete { get; set; }

    /// <summary>
    /// Error message that appears during a get request.
    /// Null for no error.
    /// </summary>
    public string? GetRequestErrorMessage { get; private set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFileInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFileWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "DeleteFileErrorMessage" )]
    public string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( string? cid )
    {
        if( this.AllowMetadataEdit() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }
        else if( string.IsNullOrWhiteSpace( cid ) )
        {
            this.GetRequestErrorMessage = "CID not specified.";
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Page();
        }

        this.JallerFile = await Task.Run( () => this.core.Files.TryGetFile( cid ) );

        if( this.JallerFile is null )
        {
            this.GetRequestErrorMessage = "Can not find file at the specified CID.";
            this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        this.InfoMessage = null;
        this.WarningMessage = null;
        this.ErrorMessage = null;

        if( this.AllowMetadataEdit() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }
        else if( string.IsNullOrWhiteSpace( this.FileToDelete ) )
        {
            this.ErrorMessage = "CID not specified.";
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return RedirectToPage();
        }

        try
        {
            await Task.Run( () => this.core.Files.DeleteFile( this.FileToDelete ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        return RedirectToPage( "FileDeleteConfirmation", new { cid = this.FileToDelete } );
    }
}
