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
using Jaller.Standard.Search;
using Microsoft.AspNetCore.Mvc;

namespace Jaller.Server.Pages.Search;

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

    public IReadOnlyCollection<JallerSearchResult>? FoundFiles { get; private set; }

    public string? Query { get; private set; }

    public string? ErrorMessage { get; private set; }

    // ---------------- Methods ----------------

    public async Task<IActionResult> OnGetAsync( string? query )
    {
        if( query is null )
        {
            return Page();
        }

        this.Query = query;
        try
        {
            this.FoundFiles = await Task.Run( () => this.core.Search.Search( query ) );
        }
        catch( Exception e )
        {
            this.ErrorMessage = e.Message;
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        return Page();
    }
}
