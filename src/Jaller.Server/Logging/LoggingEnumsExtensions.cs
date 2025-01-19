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

using Jaller.Standard.Logging;
using Serilog.Events;

namespace Jaller.Server.Logging;

public static class LoggingEnumsExtensions
{
    public static LogEventLevel ToSerilogLevel( this JallerLogLevel level ) => level switch
    {
        JallerLogLevel.Verbose => LogEventLevel.Verbose,
        JallerLogLevel.Debug => LogEventLevel.Debug,
        JallerLogLevel.Information => LogEventLevel.Information,
        JallerLogLevel.Warning => LogEventLevel.Warning,
        JallerLogLevel.Error => LogEventLevel.Error,
        JallerLogLevel.Fatal => LogEventLevel.Fatal,
        _ => throw new ArgumentException( $"Unknown log level: {level}", nameof( level ) )
    };
}
