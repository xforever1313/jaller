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
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages;

public sealed class FileModel : BasePageModel
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public FileModel( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public JallerFile? JallerFile { get; private set; }

    public string? CIDV0 { get; private set; }

    public string? ErrorMessage { get; private set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGet( string? cid )
    {
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

        return Page();
    }
}
