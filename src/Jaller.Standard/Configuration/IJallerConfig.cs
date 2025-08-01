﻿//
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

namespace Jaller.Standard.Configuration;

public interface IJallerConfig
{
    // ---------------- Properties ----------------

    IJallerDatabaseConfig Database { get; }

    IJallerIpfsGatewayConfig Ipfs { get; }

    IJallerLoggingConfig Logging { get; }

    IJallerMonitoringConfig Monitoring { get; }

    IJallerSearchConfig Search { get; }

    IJallerUserConfig Users { get; }

    IJallerWebConfig Web { get; }

    // ---------------- Methods ----------------

    /// <summary>
    /// Goes through the config and checks to see what is valid and what is not.
    /// </summary>
    /// <returns>
    /// An empty list if nothing is invalid.  Otherwise, errors with the config.
    /// </returns>
    IList<string> TryValidate( ICronStringValidator cronStringValidator );
}
