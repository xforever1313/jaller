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

namespace Jaller.Standard.Configuration;

public interface IJallerWebConfig
{
    /// <summary>
    /// A list of allowed admin URLs.  If someone tries to login
    /// to the admin interface and it is not prefixed with one of these URLs,
    /// they will not be allowed in.
    /// 
    /// The use case is if Jaller is running on the public internet, but it is desired
    /// to make the admin interface appear only on a local network.
    /// 
    /// Set to null to have no restrictions.  An empty array means the Admin
    /// interface will full-stop not work.
    /// </summary>
    IReadOnlyCollection<string>? AllowedAdminHosts { get; }

    /// <summary>
    /// What is passed into ASP.NET Core Urls Setting.
    /// See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-8.0
    /// for more information.
    /// 
    /// Set to null to not set the setting.  This may be useful if you want to use Environment
    /// Variables or a JSON file instead to set this setting.
    /// </summary>
    IReadOnlyCollection<string>? AspNetCoreUrls { get; }

    /// <summary>
    /// If set to false, a URL that contains a port number
    /// will be rejected.  This should generally be set to
    /// true in a development environment or if behind
    /// a reverse proxy.
    /// </summary>
    bool AllowPortsInUrl { get; }
    
    /// <summary>
    /// If set to true, this will have metrics appear on /Metrics.
    /// If set to false, no metrics will appear at /Metrics.
    /// </summary>
    bool EnableMetrics { get; }
    
    /// <summary>
    /// If set to true, if the requested URL that contains "//" this will
    /// rewrite the URL so each "//" becomes "/" instead.
    /// </summary>
    bool RewriteDoubleSlashes { get; }

    /// <summary>
    /// The base path at which the application runs.
    /// It is where the runtime looks for the executables and libraries.
    /// 
    /// Leave null to use the default location.
    /// </summary>
    string? ContentRoot { get; }

    /// <summary>
    /// Where the static files are located.  Set to null to use the ASP.NET Core
    /// default location.
    /// </summary>
    string? WebRoot { get; }
}
