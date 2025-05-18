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

using System.Net.Http.Headers;
using Jaller.Core.Bulk;
using Jaller.Core.Database;
using Jaller.Core.FileManagement;
using Jaller.Core.FolderManagement;
using Jaller.Core.Ipfs;
using Jaller.Core.Search;
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Jaller.Standard.Configuration;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Standard.Ipfs;
using Jaller.Standard.Logging;
using Jaller.Standard.Search;
using Jaller.Standard.UserManagement;

namespace Jaller.Core
{
    public sealed class JallerCore : IJallerCore, IDisposable
    {
        // ---------------- Fields ----------------

        private readonly HttpClient ipfsGatewayClient;

        // ---------------- Constructor ----------------

        public JallerCore( IJallerConfig config, IJallerLogger log )
        {
            this.Config = config;
            this.Log = log;

            this.ipfsGatewayClient = new HttpClient
            {
                BaseAddress = config.Ipfs.KuboUrl
            };
            this.ipfsGatewayClient.DefaultRequestHeaders.UserAgent.Clear();
            this.ipfsGatewayClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    "Jaller",
                    this.GetType().Assembly.GetName()?.Version?.ToString( 3 ) ?? "0.0.0"
                )
            );

            this.Database = new JallerDatabase( this.Config );
            this.SearchCache = new JallerSearchCache( this.Config );

            this.BulkOperations = new JallerBulkOperations( this );
            this.Files = new JallerFileManager( this, this.Database );
            this.Folders = new JallerFolderManager( this, this.Database );
            this.Ipfs = new JallerIpfsManager( this, this.ipfsGatewayClient );
            this.Search = new JallerSearcher( this, this.SearchCache, this.Database );
        }

        // ---------------- Properties ----------------

        public IJallerBulkOperations BulkOperations { get; }

        public IJallerConfig Config { get; }

        public IJallerFolderManager Folders { get; }

        public IJallerIpfsManager Ipfs { get; }

        public IJallerLogger Log { get; }

        public IJallerFileManager Files { get; }

        public IJallerSearch Search { get; }

        public IUserManager UserManager => throw new NotImplementedException();

        internal JallerDatabase Database { get; }

        internal JallerSearchCache SearchCache { get; }

        // ---------------- Methods ----------------

        public void Init()
        {
            this.Search.Index( CancellationToken.None );
        }

        public void Dispose()
        {
            this.Database.Dispose();
            this.SearchCache.Dispose();
            this.ipfsGatewayClient.Dispose();
        }
    }
}
