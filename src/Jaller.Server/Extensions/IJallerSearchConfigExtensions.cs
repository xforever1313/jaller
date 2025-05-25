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
using Jaller.Standard.Configuration;
using Jaller.Standard.Logging;
using Quartz;

namespace Jaller.Server.Extensions;

internal static class IJallerSearchConfigExtensions
{
    public static string GetUpdateString( this IJallerSearchConfig config, IJallerLogger log )
    {
        string defaultIndexRate = new JallerSearchConfig().IndexUpdateRate;

        if( string.IsNullOrWhiteSpace( config.IndexUpdateRate ) )
        {
            log.Warning( $"Warning, search index update rate set not set.  Defaulting to: {defaultIndexRate}." );
            return defaultIndexRate;
        }
        else if( CronExpression.IsValidExpression( config.IndexUpdateRate ) == false )
        {
            log.Warning( $"Warning, search cront string is invalid.  Falling back to default of: {defaultIndexRate}." );
            return defaultIndexRate;
        }

        return config.IndexUpdateRate;
    }
}
