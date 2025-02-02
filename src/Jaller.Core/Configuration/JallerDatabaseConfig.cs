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

using Jaller.Standard.Configuration;

namespace Jaller.Core.Configuration;

public sealed record class JallerDatabaseConfig : IJallerDatabaseConfig
{
    // ---------------- Constructor ----------------

    public JallerDatabaseConfig()
    {
        this.DatabaseLocation = new FileInfo(
            Path.Combine(
                JallerConfig.DefaultPersistenceDirectory.FullName,
                "jaller.ldb"
            )
        );

        this.SharedConnection = false;
        this.AutoRebuild = false;
        this.AutoUpgradeDb = false;
        this.EncryptionPassword = null;
    }

    // ---------------- Properties ----------------

    public FileInfo? DatabaseLocation { get; set; }

    public bool SharedConnection { get; set; }

    public bool AutoRebuild { get; set; }

    public bool AutoUpgradeDb { get; set; }

    public string? EncryptionPassword { get; set; }
}
