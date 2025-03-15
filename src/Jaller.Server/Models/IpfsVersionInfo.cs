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

using System.Text.Json.Serialization;

namespace Jaller.Server.Models;

public sealed class IpfsVersionInfo
{
    // ---------------- Properties ----------------

    [JsonPropertyName( "Commit" )]
    public string? Commit { get; init; }

    [JsonPropertyName( "Golang" )]
    public string? Golang { get; init; }

    [JsonPropertyName( "Repo" )]
    public string? Repo { get; init; }

    [JsonPropertyName( "System" )]
    public string? System { get; init; }

    [JsonPropertyName( "Version" )]
    public string? Version { get; init; }
}
