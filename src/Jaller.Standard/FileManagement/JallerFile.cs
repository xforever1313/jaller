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

namespace Jaller.Standard.FileManagement;

public sealed record class JallerFile
{
    /// <summary>
    /// The content ID (CID) of the file.
    /// This must be version 1 of IPFS's CID hashing protocol.
    /// </summary>
    public required string CidV1 { get; init; }

    /// <summary>
    /// The parent folder folder's ID.  Null if this goes inside of the root folder.
    /// </summary>
    public required int? ParentFolder { get; init; }

    public required string Name { get; init; } = "Untitled";

    public string? Description { get; init; } = null;

    /// <summary>
    /// Overloads the file name's mime type.  Set to null
    /// to base the mime type off of the file name's extension.
    /// </summary>
    public string? MimeType { get; init; } = null;

    public FileMetadataPolicy MetadataPrivacy { get; init; } = FileMetadataPolicy.Private;

    public FileDownloadPolicy DownloadablePolicy { get; init; } = FileDownloadPolicy.Private;
}
