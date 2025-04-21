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

using System.Security.Claims;
using Jaller.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Tests;

public static class PageModelExtensions
{
    // ---------------- Methods ----------------

    public static PageModel UseCore( this PageModel model, IJallerCore core )
    {
        return model.UseCore( core, null );
    }

    public static PageModel UseCore( this PageModel model, IJallerCore core, ClaimsIdentity? identity )
    {
        var httpContext = new DefaultHttpContext().UseCore( core );

        if( identity is not null )
        {
            httpContext.User = new ClaimsPrincipal( identity );
        }

        model.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        Assert.IsNotNull( model.HttpContext, "HTTP Context is null!");
        Assert.IsNotNull( model.HttpContext.RequestServices, "Request services are null!" );

        return model;
    }
}