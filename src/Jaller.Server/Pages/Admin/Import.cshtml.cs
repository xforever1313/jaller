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
using System.Xml.Linq;
using Jaller.Server.Extensions;
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Microsoft.AspNetCore.Mvc;
using SethCS.Extensions;

namespace Jaller.Server.Pages.Admin
{
    public class ImportModel : BasePageModel, IAlert
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public ImportModel( IJallerCore core ) :
            base( core )
        {
            this.core = core;

            this.OverwriteExistingFiles = false;
        }

        // ---------------- Properties ----------------

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        [BindProperty]
        public bool OverwriteExistingFiles { get; set; }

        /// <inheritdoc/>
        [TempData( Key = "ImportInfoMessage" )]
        public string? InfoMessage { get; set; }

        /// <inheritdoc/>
        [TempData( Key = "ImportWarningMessage" )]
        public string? WarningMessage { get; set; }

        /// <inheritdoc/>
        [TempData( Key = "ImportErrorMessage" )]
        public string? ErrorMessage { get; set; }

        // ---------------- Methods ----------------

        public IActionResult OnGet()
        {
            if( this.core.Config.Web.IsAdminRequstAllowed( this.Request ) == false )
            {
                return StatusCode( (int)HttpStatusCode.Forbidden );
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if( this.core.Config.Web.IsAdminRequstAllowed( this.Request ) == false )
            {
                return StatusCode( (int)HttpStatusCode.Forbidden );
            }

            var model = new Models.ImportModel( this.UploadedFile, this.OverwriteExistingFiles );

            string? validationMessage = model.TryValidate();
            if( string.IsNullOrEmpty( validationMessage ) == false )
            {
                this.ErrorMessage = validationMessage;
                return RedirectToPage();
                    
            }
            else if( model.File is null )
            {
                this.ErrorMessage = "File was somehow null.";

                return RedirectToPage();
            }

            await using var stream = model.File.OpenReadStream();
            XDocument doc;
            try
            {
                doc = await XDocument.LoadAsync( stream, LoadOptions.None, CancellationToken.None );
            }
            catch( Exception e )
            {
                this.ErrorMessage = e.Message;
                return RedirectToPage();
            }

            BulkAddResult result = await Task.Run(
                () => this.core.BulkOperations.BulkAddMetaData( doc, model.OverwriteExistingFiles )
            );

            this.WarningMessage = result.Warnings.Any() ? result.Warnings.ToListString() : null;
            this.ErrorMessage = result.Errors.Any() ? result.Errors.ToListString() : null;

            if( ( this.WarningMessage is null ) && ( this.ErrorMessage is null ) )
            {
                this.InfoMessage = "Import Successful!";
                this.WarningMessage = null;
                this.ErrorMessage = null;
            }

            return RedirectToPage();
        }
    }
}
