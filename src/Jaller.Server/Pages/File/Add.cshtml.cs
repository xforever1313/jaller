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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Jaller.Server.Pages.File;

public sealed class AddModel : FileAddEditModel
{
    // ---------------- Constructor ----------------

    public AddModel( IJallerCore core ) :
        base( core )
    {
    }

    // ---------------- Properties ----------------

    [BindNever]
    public JallerFolder? ParentFolder { get; private set; }

    // -------- Messages --------

    /// <inheritdoc/>
    [TempData( Key = "AddFileInfoMessage" )]
    public override string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddFileWarningMessage" )]
    public override string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddFileErrorMessage" )]
    public override string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( int? parentFolderId )
    {
        if( this.AllowMetadataEdit() == false )
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

        if( this.AllowMetadataEdit() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        JallerFile? file = ToJallerFile();
        if( file is null )
        {
            return RedirectToPage();
        }

        try
        {
            await Task.Run( () => this.core.Files.ConfigureFile( file ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        return RedirectToPage( "Index", new { cid = file.CidV1, redirectFrom = IndexModel.RedirectFrom.Add }  );
    }
}