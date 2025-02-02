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

using Jaller.Core.Configuration;
using Jaller.Core.Database;
using Jaller.Standard.Configuration;
using Jaller.Standard.Logging;
using LiteDB;

namespace Jaller.Core
{
    public sealed class JallerCore : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly JallerDatabase database;

        // ---------------- Constructor ----------------

        public JallerCore( IJallerConfig config, IJallerLogger log )
        {
            this.Config = config;
            this.database = new JallerDatabase( this.Config );

            this.Log = log;
        }

        // ---------------- Properties ----------------

        public IJallerConfig Config { get; }

        public IJallerLogger Log { get; }

        // ---------------- Methods ----------------

        public void Init()
        {
        }

        public void Dispose()
        {
            this.database.Dispose();
        }
    }
}
