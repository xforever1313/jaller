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
using Jaller.Server.Models;
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Folder;

public sealed class IndexModel : BasePageModel
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public IndexModel( IJallerCore core ) :
        base( core )
    {
        this.core = core;
    }

    // ---------------- Properties ----------------

    public JallerFolder? JallerFolder { get; private set; }

    public FolderContents? FolderContents { get; private set; }

    public string? ErrorMessage { get; private set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( int? id )
    {
        // If no ID is specified, assume root directory.
        id = id ?? 0;

        if( id == 0 )
        {
            // Make a dummy folder to represent the root directory.
            this.JallerFolder = new JallerFolder
            {
                // Root folder is always public.
                DownloadablePolicy = DownloadPolicy.Public,
                MetadataPrivacy = MetadataPolicy.Public,
                Name = "Root",
                ParentFolder = null
            };
        }
        else
        {
            this.JallerFolder = await Task.Run( () => this.core.Folders.TryGetFolder( id.Value ) );
        }
        
        if( this.JallerFolder is null )
        {
            this.ErrorMessage = "Can not find folder at the specified ID.";
            this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Page();
        }

        if( id == 0 )
        {
            this.FolderContents = await Task.Run( () => this.core.Folders.GetRootFolder( MetadataPolicy.Public ) );
        }
        else
        {
            this.FolderContents = await Task.Run( () => this.core.Folders.TryGetFolderContents( id.Value, MetadataPolicy.Public ) );
        }

        return Page();
    }
}
