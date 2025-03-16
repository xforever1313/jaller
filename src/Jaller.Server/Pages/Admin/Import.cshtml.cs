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

using System.Xml.Linq;
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Admin
{
    public class ImportModel : BasePageModel, IAlert
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public ImportModel( IJallerCore core )
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
        public IEnumerable<string>? InfoMessages { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<string>? WarningMessages { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<string>? ErrorMessages { get; private set; }

        // ---------------- Methods ----------------

        public void OnGet(
            IEnumerable<string>? infoMessages = null,
            IEnumerable<string>? warningMessages = null,
            IEnumerable<string>? errorMessages = null
        )
        {
            this.InfoMessages = infoMessages;
            this.WarningMessages = warningMessages;
            this.ErrorMessages = errorMessages;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var model = new Models.ImportModel( this.UploadedFile, this.OverwriteExistingFiles );

            string? validationMessage = model.TryValidate();
            if( string.IsNullOrEmpty( validationMessage ) == false )
            {
                this.ErrorMessages = [validationMessage];
                return RedirectToPage(
                    "/Admin/Import",
                    new
                    {
                        this.InfoMessages,
                        this.WarningMessages,
                        this.ErrorMessages
                    }
                );
                    
            }
            else if( model.File is null )
            {
                this.ErrorMessages = ["File was somehow null."];

                return RedirectToPage(
                    "/Admin/Import",
                    new
                    {
                        this.InfoMessages,
                        this.WarningMessages,
                        this.ErrorMessages
                    }
                );
            }

            await using var stream = model.File.OpenReadStream();
            XDocument doc;
            try
            {
                doc = await XDocument.LoadAsync( stream, LoadOptions.None, CancellationToken.None );
            }
            catch( Exception e )
            {
                this.ErrorMessages = [e.Message];
                return RedirectToPage(
                    "/Admin/Import",
                    new
                    {
                        this.InfoMessages,
                        this.WarningMessages,
                        this.ErrorMessages
                    }
                );
            }

            BulkAddResult result = await Task.Run(
                () => this.core.BulkOperations.BulkAddMetaData( doc, model.OverwriteExistingFiles )
            );

            this.WarningMessages = result.Warnings.Any() ? result.Warnings : null;
            this.ErrorMessages = result.Errors.Any() ? result.Errors : null;

            if( ( this.WarningMessages is null ) && ( this.ErrorMessages is null ) )
            {
                this.InfoMessages = ["Import Successful!"];
            }

            return RedirectToPage(
                "/Admin/Import",
                new
                {
                    this.InfoMessages,
                    this.WarningMessages,
                    this.ErrorMessages
                }
            );
        }
    }
}
