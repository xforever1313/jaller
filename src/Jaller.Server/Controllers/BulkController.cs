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
using Jaller.Contracts.Bulk;
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc;
using SethCS.Extensions;

namespace Jaller.Server.Controllers;

[Route( "api/bulk" )]
[ApiController]
public sealed class BulkController : ControllerBase
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;
    
    // ---------------- Constructor ----------------

    public BulkController( IJallerCore core )
    {
        this.core = core;
    }
    
    // ---------------- Methods ----------------

    [HttpGet( "jaller.xml" )]
    public async Task<IActionResult> Download()
    {
        this.HttpContext.Response.ContentType = "application/xml";

        // TODO: Allow this to be private based on
        // if a user is logged in.
        XDocument doc = await Task.Run(
            () => this.core.BulkOperations.BulkGetAllMetaData( MetadataPolicy.MachinePublic )
        );
        
        return Ok( doc.ToString() );
    }

    [HttpPost( "import.xml" )]
    public async Task<IActionResult> Import( [FromForm] ImportModel model )
    {
        if( "POST".EqualsIgnoreCase( this.Request.Method ) == false )
        {
            return BadRequest( "This must be a POST request." );
        }

        await using var stream = model.File.OpenReadStream();
        XDocument doc;
        try
        {
            doc = await XDocument.LoadAsync( stream, LoadOptions.None, CancellationToken.None );
        }
        catch( Exception e )
        {
            return BadRequest( e.Message );
        }
        
        BulkAddResult result = await Task.Run(
            () => this.core.BulkOperations.BulkAddMetaData( doc, model.OverwriteExistingFiles ) 
        );

        var response = new ImportResult( result.Warnings, result.Errors );
        return Ok( response );
    }
}