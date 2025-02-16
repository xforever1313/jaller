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

using Jaller.Standard;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Controllers;

[Route( "api/ipfs_info" )]
[ApiController]
public sealed class IpfsInfoController : ControllerBase
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public IpfsInfoController( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    [HttpGet( "version.json" )]
    public async Task<IActionResult> Version()
    {
        Stream stream = await Task.Run(
            () => this.core.Ipfs.GetVersionInfo()
        );

        this.HttpContext.Response.ContentType = "application/json";

        return Ok( stream );
    }
}
