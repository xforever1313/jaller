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

using Jaller.Core.Database;
using Jaller.Core.FileManagement;
using Jaller.Core.FolderManagement;
using Jaller.Standard;
using Jaller.Standard.Configuration;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Standard.Logging;
using Jaller.Standard.UserManagement;

namespace Jaller.Core
{
    public sealed class JallerCore : IJallerCore, IDisposable
    {
        // ---------------- Constructor ----------------

        public JallerCore( IJallerConfig config, IJallerLogger log )
        {
            this.Config = config;
            this.Database = new JallerDatabase( this.Config );

            this.Files = new JallerFileManager( this, this.Database );
            this.Folders = new FolderManager( this, this.Database );

            this.Log = log;
        }

        // ---------------- Properties ----------------

        public IJallerConfig Config { get; }

        public IFolderManager Folders { get; }

        public IJallerLogger Log { get; }

        public IJallerFileManager Files { get; }

        public IUserManager UserManager => throw new NotImplementedException();

        internal JallerDatabase Database { get; }

        // ---------------- Methods ----------------

        public void Init()
        {
        }

        public void Dispose()
        {
            this.Database.Dispose();
        }
    }
}
