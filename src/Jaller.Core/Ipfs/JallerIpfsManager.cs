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
using Jaller.Standard.Ipfs;

namespace Jaller.Core.Ipfs
{
    internal sealed class JallerIpfsManager : IJallerIpfsManager
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;
        private readonly HttpClient httpClient;

        private const string apiPath = "/api/v0";

        // ---------------- Constructor ----------------

        public JallerIpfsManager( IJallerCore core, HttpClient client )
        {
            this.core = core;
            this.httpClient = client;
        }

        // ---------------- Methods ----------------

        public Stream GetVersionInfo()
        {
            const string versionPath = apiPath + "/version?all=true";

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri( versionPath, UriKind.Relative ),
                Method = HttpMethod.Post
            };

            HttpResponseMessage response = this.httpClient.Send( request );
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStream();
        }

        public Stream GetFile( string cid )
        {
            string versionPath = apiPath + $"/cat?progress=false&arg={cid}";
            
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri( versionPath, UriKind.Relative ),
                Method = HttpMethod.Post
            };

            HttpResponseMessage response = this.httpClient.Send( request );
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStream();
        }
    }
}
