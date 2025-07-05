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

public interface IJallerMonitoringConfig
{
    /// <summary>
    /// A list of files that Jaller will look to see if they exist or not.
    /// If any file can not be read, a <see cref="Logging.JallerLogLevel.Fatal"/> message is generated.
    /// If the file comes back, another <see cref="Logging.JallerLogLevel.Fatal"/> message saying it is all
    /// clear will be generated.
    /// 
    /// The use case for this is to detect if a drive where IPFS files or the Jaller database
    /// files are located goes down.
    /// </summary>
    IEnumerable<FileInfo>? CanaryFiles { get; }

    /// <summary>
    /// A cron-string to determine how often to check the <see cref="CanaryFiles"/>.
    /// 
    /// Ignored if <see cref="CanaryFiles"/> is null or empty.
    /// See https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
    /// for more information on how to make this string.
    ///
    /// Defaulted to every 10 minutes.
    /// </summary>
    string CanaryCheckRate { get; }
}
