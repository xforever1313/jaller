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
using Jaller.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Tests.Pages.Admin
{
    public static class CommonAdminUnitTests
    {
        /// <summary>
        /// Ensures the host is restricted based on the configuration.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="config"></param>
        public static void DoHostRestrictionTest( Func<IActionResult> action, PageModel model, JallerWebConfig config )
        {
            var httpContext = new DefaultHttpContext();
            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            var urlsToTry = new string[]
            {
                "localhost",
                "ipfs.shendrick.net"
            };

            // Empty array means admin panel is basically disabled.
            config.AllowedAdminHosts = [];

            foreach( string url in urlsToTry )
            {
                model.Request.Host = new HostString( url );

                IActionResult result = action();
                StatusCodeResult? statusCodeResult = result as StatusCodeResult;
                Assert.IsNotNull( statusCodeResult );

                Assert.AreEqual( (int)HttpStatusCode.Forbidden, statusCodeResult.StatusCode );
            }

            config.AllowedAdminHosts = ["somewhere.shendrick.net"];

            foreach( string url in urlsToTry )
            {
                model.Request.Host = new HostString( url );

                IActionResult result = action();
                StatusCodeResult? statusCodeResult = result as StatusCodeResult;
                Assert.IsNotNull( statusCodeResult );

                Assert.AreEqual( (int)HttpStatusCode.Forbidden, statusCodeResult.StatusCode );
            }
        }
    }
}
