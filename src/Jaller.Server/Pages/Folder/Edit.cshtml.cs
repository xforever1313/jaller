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

namespace Jaller.Server.Pages.Folder;

public sealed class EditModel : BasePageModel, IAlert, IJallerPermissions
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

    // -------- POST Properties --------

    [BindProperty]
    public int? FolderId { get; set; }

    [BindProperty]
    public int? ParentFolderId { get; set; }

    [BindProperty]
    public string? NewFolderName { get; set; }

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
        if( this.AllowMetadataEdit == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        // If no ID is specified, assume root directory.
        id = id ?? 0;

        if( id == 0 )
        {
            this.GetRequestErrorMessage = "Can not edit root folder.";
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Page();
        }

        this.JallerFolder = await Task.Run( () => this.core.Folders.TryGetFolder( id.Value ) );

        if( this.JallerFolder is null )
        {
            this.GetRequestErrorMessage = "Can not find folder at the specified ID.";
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

        if( this.AllowMetadataEdit == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        if( this.FolderId is null )
        {
            this.ErrorMessage = "Folder ID is null, it must be specified (we can not edit the root folder).";
            return RedirectToPage();
        }
        else if( this.FolderId == 0 )
        {
            this.ErrorMessage = "Can not edit the root folder.";
            return RedirectToPage();
        }

        int folderId;
        try
        {
            this.JallerFolder = await Task.Run( () => this.core.Folders.TryGetFolder( this.FolderId.Value ) );
            if( this.JallerFolder is null )
            {
                this.ErrorMessage = "Folder no longer exists in the database.  It may have been deleted before an edit took place.";
                return RedirectToPage();
            }

            int? parentFolder = this.ParentFolderId;
            if( parentFolder == 0 )
            {
                parentFolder = null;
            }

            this.JallerFolder = this.JallerFolder with
            {
                DownloadablePolicy = this.DownloadablePolicy ?? JallerFolder.DefaultDownloadablePolicy,
                MetadataPrivacy = this.MetadataPrivacy ?? JallerFolder.MetadataPrivacy,
                Name = this.NewFolderName ?? JallerFolder.DefaultFolderName,
                ParentFolder = parentFolder
            };

            folderId = await Task.Run( () => this.core.Folders.ConfigureFolder( this.JallerFolder ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        this.InfoMessage = $"Folder Updated!  Its ID is {folderId}.";

        return RedirectToPage( new { id = folderId } );
    }
}
