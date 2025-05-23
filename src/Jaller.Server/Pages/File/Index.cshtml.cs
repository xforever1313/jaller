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
using Jaller.Core.FileManagement;
using Jaller.Server.Extensions;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Server.Pages.File;

public sealed class IndexModel : PageModel
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;


    // ---------------- Constructor ----------------

    public IndexModel( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public string? RedirectMessage { get; private set; }

    public JallerFile? JallerFile { get; private set; }

    public IReadOnlyList<JallerFolder>? FolderPath { get; private set; }

    public string? CIDV0 { get; private set; }

    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// If the document type is renderable in html, this is set to not null
    /// so we can have a preview in the webpage.
    /// </summary>
    public RenderableMimeType? PreviewDocumentType { get; private set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGet( string? cid, RedirectFrom? redirectFrom )
    {
        this.RedirectMessage = redirectFrom switch
        {
            null => this.RedirectMessage = null,
            RedirectFrom.Add => "File Metadata Added!",
            RedirectFrom.Edit => "File Metadata Edited!",
            _ => this.RedirectMessage = null
        };

        // TODO: Make proper visibility
        const MetadataPolicy visibility = MetadataPolicy.Public;

        if( cid is null )
        {
            this.ErrorMessage = "No CID specified.  Please include a CID at the end of the URL.";
            this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Page();
        }

        this.JallerFile = await Task.Run( () => this.core.Files.TryGetFile( cid ) );

        if( this.JallerFile is null )
        {
            this.ErrorMessage = "Can not find CID";
            this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Page();
        }

        Cid? realCid = Cid.TryParse( this.JallerFile.CidV1 );
        if( realCid is null )
        {
            this.ErrorMessage = "Invalid or unparsable CID.";
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Page();
        }

        this.CIDV0 = realCid.Version0Cid;

        string mimeType = this.JallerFile.GetMimeType();
        string? mimeCategory = Path.GetDirectoryName( mimeType );

        // TODO: Need to handle private downloads.
        if( this.JallerFile.IsDownloadable( this.User ) )
        {
            this.PreviewDocumentType = this.JallerFile.IsRenderable();
        }

        this.FolderPath = await Task.Run( () => this.core.Files.TryGetFolderPath( this.JallerFile, visibility ) );

        return Page();
    }

    // ---------------- Helper Enums ----------------

    /// <summary>
    /// Where the page was redirected from.
    /// </summary>
    public enum RedirectFrom : byte
    {
        /// <summary>
        /// Redirected to after adding the file.
        /// </summary>
        Add = 1,

        /// <summary>
        /// Redirected to after editing the file.
        /// </summary>
        Edit = 2
    }
}
