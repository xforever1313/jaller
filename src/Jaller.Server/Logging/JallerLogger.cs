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

using Jaller.Standard.Logging;

namespace Jaller.Server.Logging;

internal sealed class JallerLogger : IJallerLogger
{
    // ---------------- Fields ----------------

    private readonly Serilog.ILogger log;

    // ---------------- Constructor ----------------

    public JallerLogger( Serilog.ILogger log )
    {
        this.log = log;
    }

    // ---------------- Methods ----------------

    public void Debug( string message )
    {
        this.log.Debug( message );
    }

    public void Verbose( string message )
    {
        this.log.Verbose( message );
    }

    public void Information( string message )
    {
        this.log.Information( message );
    }

    public void Warning( string message )
    {
        this.log.Warning( message );
    }

    public void Error( string message )
    {
        this.log.Error( message );
    }

    public void Fatal( string message )
    {
        this.log.Fatal( message );
    }
}
