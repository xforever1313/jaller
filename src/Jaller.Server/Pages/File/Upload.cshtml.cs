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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Server.Pages.File;

public sealed class UploadModel : PageModel, IAlert
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public UploadModel( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    [BindNever]
    public JallerFolder? ParentFolder { get; private set; }

    /// <summary>
    /// ID of where to upload the file to.
    /// </summary>
    [BindProperty]
    public int? ParentFolderId { get; set; }

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    // -------- Messages --------

    /// <summary>
    /// Error message that appears during a get request.
    /// Null for no error.
    /// </summary>
    [BindNever]
    public string? GetRequestErrorMessage { get; private set; }

    /// <inheritdoc/>
    [TempData( Key = "UploadFileInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "UploadFileWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "UploadFileErrorMessage" )]
    public string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( int? parentFolderId )
    {
        if( this.AllowFileUpload() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

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

        if( this.AllowFileUpload() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }
        else if( this.UploadedFile is null )
        {
            this.ErrorMessage = "Uploaded file not specified.";
            return RedirectToPage();
        }

        await Task.Delay( 0 );

        this.ErrorMessage = "Not Implemented Yet";
        return RedirectToPage();
    }
}
