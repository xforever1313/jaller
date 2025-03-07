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

public record class JallerFileInfo
{
    // ---------------- Properties ----------------

    [JsonRequired]
    [JsonPropertyName( "cidv1" )]
    public required string CidV1 { get; init; }

    [JsonPropertyName( "parent_folder" )]
    public int? ParentFolderId { get; init; }

    [JsonPropertyName( "name" )]
    public string Name { get; init; } = "Untitled";

    [JsonPropertyName( "description" )]
    public string? Description { get; init; }

    [JsonPropertyName( "mime_type" )]
    public string? MimeType { get; init; }

    [JsonPropertyName( "tags" )]
    public string[]? Tags { get; init; }
}
