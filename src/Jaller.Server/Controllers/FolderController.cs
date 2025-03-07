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

using Jaller.Server.Extensions;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Controllers
{
    [Route( "api/folders" )]
    [ApiController]
    public class FolderController : Controller
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public FolderController( IJallerCore core )
        {
            this.core = core;
        }

        // ---------------- Methods ----------------

        [HttpGet( "contents" )]
        public async Task<IActionResult> GetContents( [FromQuery] int? folderId )
        {
            FolderContents? contents = null;
            if( folderId is null )
            {
                contents = await Task.Run( () => this.core.Folders.GetRootFolder( MetadataPolicy.Public ) );
            }
            else
            {
                contents = await Task.Run( () => this.core.Folders.TryGetFolderContents( folderId.Value, MetadataPolicy.Public ) );
            }

            if( contents is null )
            {
                return NotFound( $"Could not find folder with ID: {folderId}" );
            }
            else
            {
                return Ok( contents.ToFolderContentsInfo( folderId ) );
            }
        }
    }
}
