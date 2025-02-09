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

using System.ComponentModel.DataAnnotations;
using Jaller.Standard.FileManagement;
using LiteDB;

namespace Jaller.Core.Database;

internal sealed record class IpfsFile
{
    // ---------------- Properties ----------------

    [BsonId( false )]
    [Required]
    public required string Cid { get; init; }

    public int? ParentFolder { get; init; }

    /// <summary>
    /// The name of the file.
    /// </summary>
    public string FileName { get; init; } = "Untitled";

    /// <summary>
    /// Optional description of the file.
    /// </summary>
    public string? Description { get; init; } = null;

    /// <summary>
    /// Overload the mime type of the file.  If set to null,
    /// the default mime type is used.
    /// </summary>
    public string? MimeType { get; init; } = null;

    public FileMetadataPolicy MetadataPrivacy { get; init; } = FileMetadataPolicy.Private;

    public FileDownloadPolicy DownloadablePolicy { get; init; } = FileDownloadPolicy.Private;

    /// <summary>
    /// Tags for this file, if any.
    /// Set to null if no tags are associated with this file.
    /// </summary>
    public HashSet<string>? Tags { get; init; }
}
