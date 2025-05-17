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

namespace Jaller.Server.Models;

public sealed record class Alert : IAlert
{
    // ---------------- Properties ----------------

    /// <summary>
    /// Optional heading that appears with the information message.
    /// </summary>
    public string? InfoHeading { get; init; }

    /// <inheritdoc/>
    public string? InfoMessage { get; init; }

    /// <summary>
    /// Optional heading that appears with the warning message.
    /// </summary>
    public string? WarningHeading { get; init; }

    /// <inheritdoc/>
    public string? WarningMessage { get; init; }

    /// <summary>
    /// Optional heading that appears with the error message.
    /// </summary>
    public string? ErrorHeading { get; init; }

    /// <inheritdoc/>
    public string? ErrorMessage { get; init; }
}
