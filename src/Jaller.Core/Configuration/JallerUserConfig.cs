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

public sealed record class JallerUserConfig : IJallerUserConfig
{
    // ---------------- Properties ----------------

    public bool AllowAdminUser { get; set; } = false;

    public string AdminEmail { get; set; } = "";

    /// <remarks>
    /// Empty string is not allowed, this will fail validation intentionally.
    /// We want the user to set *something*.
    /// </remarks>
    public string AdminPassword { get; set; } = "";


    public FileInfo? DatabaseLocation { get; set; } = new FileInfo(
        Path.Combine(
            JallerConfig.DefaultPersistenceDirectory.FullName,
            "jaller_users.ldb"
        )
    );

    public bool DirectConnection { get; set; } = true;

    public bool AutoRebuild { get; set; } = false;

    public bool AutoUpgradeDb { get; set; } = false;

    public string? EncryptionPassword { get; set; } = null;
}
