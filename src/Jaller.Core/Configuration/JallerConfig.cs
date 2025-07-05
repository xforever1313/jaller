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

public record class JallerConfig : IJallerConfig
{
    // ---------------- Constructor ----------------

    public JallerConfig()
    {
        this.Database = new JallerDatabaseConfig();
        this.Ipfs = new JallerIpfsGatewayConfig();
        this.Logging = new JallerLoggingConfig();
        this.Monitoring = new JallerMonitoringConfig();
        this.Search = new JallerSearchConfig();
        this.Users = new JallerUserConfig();
        this.Web = new JallerWebConfig();
    }

    static JallerConfig()
    {
        DefaultPersistenceDirectory = new DirectoryInfo(
            Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
                nameof( Jaller )
            )
        );
    }

    // ---------------- Properties ----------------

    public static DirectoryInfo DefaultPersistenceDirectory { get; }

    public JallerDatabaseConfig Database { get; }
    IJallerDatabaseConfig IJallerConfig.Database => this.Database;

    public JallerIpfsGatewayConfig Ipfs { get; }
    IJallerIpfsGatewayConfig IJallerConfig.Ipfs => this.Ipfs;

    public JallerLoggingConfig Logging { get; }
    IJallerLoggingConfig IJallerConfig.Logging => this.Logging;

    public JallerMonitoringConfig Monitoring { get; }
    IJallerMonitoringConfig IJallerConfig.Monitoring => this.Monitoring;

    public JallerSearchConfig Search { get; }
    IJallerSearchConfig IJallerConfig.Search => this.Search;

    public JallerUserConfig Users { get; }
    IJallerUserConfig IJallerConfig.Users => this.Users;

    public JallerWebConfig Web { get; }
    IJallerWebConfig IJallerConfig.Web => this.Web;

    // ---------------- Methods ----------------

    public IList<string> TryValidate( ICronStringValidator cronStringValidator )
    {
        var list = new List<string>();

        list.AddRange( this.Monitoring.TryValidate( cronStringValidator ) );

        return list;
    }
}
