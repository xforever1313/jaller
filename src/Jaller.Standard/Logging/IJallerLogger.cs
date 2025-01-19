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

namespace Jaller.Standard.Logging;

/// <summary>
/// Wrapper to an actual ILogger instance.
/// This exists so we can change
/// the back-end logging system without breaking being able to compile.
/// </summary>
public interface IJallerLogger
{
    void Debug( string message );

    void Verbose( string message );

    void Information( string message );

    void Warning( string message );

    void Error( string message );

    void Fatal( string message );
}
