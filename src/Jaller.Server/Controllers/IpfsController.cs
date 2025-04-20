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

using Jaller.Core;
using Jaller.Core.FileManagement;
using Jaller.Server.Extensions;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Microsoft.AspNetCore.Mvc;
using SethCS.Extensions;

namespace Jaller.Server.Controllers;

[Route( "ipfs" )]
[ApiController]
public sealed class IpfsController : ControllerBase
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public IpfsController( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    [HttpGet( "{cid}" )]
    public async Task<IActionResult> Download( string cid )
    {
        Cid? realCid = Cid.TryParse( cid );
        if( realCid is null )
        {
            this.Response.ContentType = "plain/text";
            return BadRequest( "Invalid or unsupported CID" );
        }

        JallerFile? file = await Getfile( realCid );
        if( file is null )
        {
            this.Response.ContentType = "plain/text";
            return NotFound( "Can not find file with given CID.  Either it does not exist, or this server does not allow one to download it." );
        }

        this.Response.ContentType = file.GetMimeType();

        return Ok( this.core.Ipfs.GetFile( realCid.Version1Cid ) );
    }

    private async Task<JallerFile?> Getfile( Cid realCid )
    {
        JallerFile? file = await Task.Run( () => this.core.Files.TryGetFile( realCid.Version1Cid ) );

        if( file.IsDownloadable( this.User ) )
        {
            return file;
        }

        return null;
    }
}
