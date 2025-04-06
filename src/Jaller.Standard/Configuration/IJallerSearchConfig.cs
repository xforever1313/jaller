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

public interface IJallerSearchConfig : IJallerDatabaseConfig
{
    /// <summary>
    /// If set to true, this will update the search index right when
    /// Jaller starts up.  However, this may delay the app from fully starting up.
    /// Set to false to not have have the search index at startup; but this may
    /// mean that the search data may be out-of-date.
    /// </summary>
    bool UpdateIndexOnStartup { get; }

    /// <summary>
    /// A cron-string on often to update the search index.
    /// 
    /// See https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
    /// for more information on how to make this string.
    ///
    /// Defaulted to every day at midnight.
    /// </summary>
    string IndexUpdateRate { get; }
}
