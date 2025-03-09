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

namespace Jaller.Contracts.FileManagement;

/// <summary>
/// Properties needed to work for the tree view in the web client.
/// </summary>
public sealed record class JallerFileTreeLeafInfo
{
    // ---------------- Properties ----------------

    [JsonRequired]
    [JsonPropertyName( "cidv1" )]
    public required string CidV1 { get; init; }

    [JsonRequired]
    [JsonPropertyName( "parent_folder" )]
    public int? ParentFolderId { get; init; }

    [JsonRequired]
    [JsonPropertyName( "name" )]
    public string Name { get; init; } = "Untitled";
}
