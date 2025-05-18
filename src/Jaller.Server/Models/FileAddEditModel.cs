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

using System.Text;
using Jaller.Core;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SethCS.Extensions;

namespace Jaller.Server.Models;

public abstract class FileAddEditModel : PageModel, IAlert, IJallerPermissions
{
    // ---------------- Fields ----------------

    protected readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public FileAddEditModel( IJallerCore core )
    {
        this.core = core;

        this.MimeTypeOverridden = false;
    }

    // ---------------- Properties ----------------

    public bool MimeTypeOverridden { get; private set; }

    // -------- POST Properties --------

    /// <summary>
    /// The CID of the file.  Can either be V0 or V1.
    /// </summary>
    [BindProperty]
    public string? Cid { get; set; }

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
    public string? Tags { get; set; }

    // -------- Messages --------

    /// <summary>
    /// Error message that appears during a get request.
    /// Null for no error.
    /// </summary>
    [BindNever]
    public string? GetRequestErrorMessage { get; protected set; }

    /// <inheritdoc/>
    public abstract string? InfoMessage { get; set; }

    /// <inheritdoc/>
    public abstract string? WarningMessage { get; set; }

    /// <inheritdoc/>
    public abstract string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    protected void FromJallerFile( JallerFile file )
    {
        this.MimeTypeOverridden = ( string.IsNullOrWhiteSpace( file.MimeType ) == false );

        this.Cid = file.CidV1;
        this.Description = file.Description;
        this.Details = file.Details;
        this.DownloadablePolicy = file.DownloadablePolicy;
        this.FileName = file.Name;
        this.MetadataPrivacy = file.MetadataPrivacy;
        this.MimeType = file.MimeType;
        this.ParentFolderId = file.ParentFolder;

        if( file.Tags?.Any() ?? false )
        {
            var tagBuilder = new StringBuilder();
            foreach( string tag in file.Tags )
            {
                tagBuilder.Append( tag + " " );
            }
            this.Tags = tagBuilder.ToString().TrimEnd();
        }

        this.Title = file.Title;
    }

    /// <summary>
    /// Converts all the bound properties to a <see cref="JallerFile"/> object.
    /// If an error happens, this writes it to <see cref="ErrorMessage"/>
    /// and this returns null.
    /// </summary>
    protected JallerFile? ToJallerFile()
    {
        if( this.ParentFolderId == 0 )
        {
            this.ParentFolderId = null;
        }

        if( string.IsNullOrWhiteSpace( this.Cid ) )
        {
            this.ErrorMessage = "CID was not specified, it is required.";
            return null;
        }

        Cid? cid = Jaller.Core.Cid.TryParse( this.Cid );
        if( cid is null )
        {
            this.ErrorMessage = "CID was invalid.";
            return null;
        }

        if( string.IsNullOrWhiteSpace( this.FileName ) )
        {
            this.ErrorMessage = "File name was not specified.";
            return null;
        }

        TagSet? tags = null;
        if( string.IsNullOrWhiteSpace( this.Tags ) == false )
        {
            string tagString = this.Tags.NormalizeWhiteSpace().ToLower();
            string[] tagSplit = tagString.Split( ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
            if( tagSplit.Any() )
            {
                tags = [.. tagSplit];
            }
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

        // If this is null, we'll attempt to determine the mime type from the
        // file name.
        // Do not override the mime type before creating the file, since we want it to be
        // null in the database to mean "do not override".
        if( string.IsNullOrWhiteSpace( this.MimeType ) )
        {
            this.MimeTypeOverridden = false;
            this.MimeType = this.FileName.GetMimeType();
        }
        else
        {
            this.MimeTypeOverridden = true;
        }

        return file;
    }
}
