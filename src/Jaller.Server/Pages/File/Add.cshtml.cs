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
using Jaller.Core;
using Jaller.Server.Extensions;
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SethCS.Extensions;

namespace Jaller.Server.Pages.File;

public class AddModel : PageModel, IAlert, IJallerPermissions
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public AddModel( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    [BindNever]
    public JallerFolder? ParentFolder { get; private set; }

    // -------- POST Properties --------

    /// <summary>
    /// The CID of the file.  Can either be V0 or V1.
    /// </summary>
    [BindProperty]
    public string? Cid { get; init; }

    /// <see cref="JallerFile.Parentfolder"/>
    [BindProperty]
    public int? ParentFolderId { get; set; }

    /// <see cref="JallerFile.Name"/>
    [BindProperty]
    public string? FileName { get; set; }

    /// <see cref="JallerFile.Title"/>
    [BindProperty]
    public string? Title { get; set; }

    /// <see cref="JallerFile.Description"/>
    [BindProperty]
    public string? Description { get; set; }

    /// <see cref="JallerFile.Details"/>
    [BindProperty]
    public string? Details { get; set; }

    /// <see cref="JallerFile.Details"/>
    [BindProperty]
    public string? MimeType { get; set; }

    /// <see cref="JallerFile.MetadataPrivacy"/>
    [BindProperty]
    public MetadataPolicy? MetadataPrivacy { get; set; }

    /// <see cref="JallerFile.DownloadablePolicy"/>
    [BindProperty]
    public DownloadPolicy? DownloadablePolicy { get; set; }

    /// <summary>
    /// Space-separated tags.
    /// </summary>
    /// <see cref="JallerFile.Tags"/>
    [BindProperty]
    public string? Tags{ get; set; }

    // -------- Messages --------

    /// <summary>
    /// Error message that appears during a get request.
    /// Null for no error.
    /// </summary>
    [BindNever]
    public string? GetRequestErrorMessage { get; private set; }

    /// <inheritdoc/>
    [TempData( Key = "AddFileInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddFileWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddFileErrorMessage" )]
    public string? ErrorMessage { get; set; }

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

        if( this.ParentFolderId == 0 )
        {
            this.ParentFolderId = null;
        }

        if( string.IsNullOrWhiteSpace( this.Cid ) )
        {
            this.ErrorMessage = "CID was not specified, it is required.";
            return RedirectToPage();
        }

        Cid? cid = Jaller.Core.Cid.TryParse( this.Cid );
        if( cid is null )
        {
            this.ErrorMessage = "CID was invalid.";
            return RedirectToPage();
        }

        if( string.IsNullOrWhiteSpace( this.FileName ) )
        {
            this.ErrorMessage = "File name was not specified.";
            return RedirectToPage();
        }

        TagSet? tags = null;
        if( string.IsNullOrWhiteSpace( this.Tags ) )
        {
            string tagString = this.Tags.NormalizeWhiteSpace().ToLower();
            string[] tagSplit = tagString.Split( ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
            if( tagSplit.Any() )
            {
                tags = new TagSet();
                foreach( string tag in tagSplit )
                {
                    if( tag.Contains( tag ) == false )
                    {
                        tags.Add( tag );
                    }
                }
            }
        }

        // If this is null, we'll attempt to determine the mime type from the
        // file name.
        if( string.IsNullOrWhiteSpace( this.MimeType ) )
        {
            this.MimeType = this.FileName.GetMimeType();
        }

        var file = new JallerFile
        {
            CidV1 = cid.Version1Cid,
            Description = this.Description,
            Details = this.Details,
            DownloadablePolicy = this.DownloadablePolicy ?? JallerFile.DefaultDownloadablePolicy,
            MetadataPrivacy = this.MetadataPrivacy ?? JallerFile.DefaultMetadataPolicy,
            MimeType = this.MimeType,
            Name = this.FileName,
            ParentFolder = this.ParentFolderId,
            Tags = tags,
            Title = this.Title
        };

        try
        {
            await Task.Run( () => this.core.Files.ConfigureFile( file ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            return RedirectToPage();
        }

        this.InfoMessage = $"File Metadata Added!";
        return RedirectToPage( "Index", new { cid = file.CidV1 }  );
    }
}