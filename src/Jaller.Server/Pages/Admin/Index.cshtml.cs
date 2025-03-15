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

using System.Text.Json;
using Jaller.Server.Models;
using Jaller.Standard;

namespace Jaller.Server.Pages.Admin
{
    public class IndexModel : BasePageModel
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        // ---------------- Constructor ----------------

        public IndexModel( IJallerCore core )
        {
            this.core = core;
        }

        // ---------------- Properties ----------------

        public int? NumberOfFiles { get; private set; }

        public int? NumberOfDirectories { get; private set; }

        public IpfsVersionInfo? IpfsVersionInfo { get; private set; }

        public string? IpfsVersionErrorString { get; private set; }

        // ---------------- Methods ----------------

        public async Task OnGetAsync()
        {
            this.NumberOfFiles = this.core.Files.GetFileCount();
            this.NumberOfDirectories = this.core.Folders.GetFolderCount();

            try
            {
                using Stream stream = await Task.Run( () => this.core.Ipfs.GetVersionInfo() );
                this.IpfsVersionInfo = await JsonSerializer.DeserializeAsync<IpfsVersionInfo>( stream );
            }
            catch( Exception e )
            {
                this.IpfsVersionErrorString = e.Message;
            }
        }
    }
}
