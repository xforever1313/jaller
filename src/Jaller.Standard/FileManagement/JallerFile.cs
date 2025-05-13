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
    // ---------------- Properties ----------------

    /// <summary>
    /// The content ID (CID) of the file.
    /// This must be version 1 of IPFS's CID hashing protocol.
    /// </summary>
    public required string CidV1 { get; init; }

    /// <summary>
    /// The parent folder folder's ID.  Null if this goes inside of the root folder.
    /// </summary>
    public required int? ParentFolder { get; init; }

    /// <summary>
    /// File name
    /// </summary>
    public required string Name { get; init; } = "Untitled";

    /// <summary>
    /// The title of the file.  This is different than the name.
    /// If this is null, it falls back to <see cref="Name"/> .
    /// </summary>
    public string? Title { get; init; } = null;

    /// <summary>
    /// A one line sentence that describes what the file is.
    /// </summary>
    public string? Description { get; init; } = null;

    /// <summary>
    /// Long details about the file.  Markdown is supported.
    /// </summary>
    public string? Details { get; init; } = null;

    /// <summary>
    /// Overloads the file name's mime type.  Set to null
    /// to base the mime type off of the file name's extension.
    /// </summary>
    public string? MimeType { get; init; } = null;

    public MetadataPolicy MetadataPrivacy { get; init; } = MetadataPolicy.Private;

    public DownloadPolicy DownloadablePolicy { get; init; } = DownloadPolicy.Private;

    /// <summary>
    /// Tags for this file, if any.
    /// Set to null if no tags are associated with this file.
    /// </summary>
    public TagSet? Tags { get; init; }

    // ---------------- Methods ----------------

    public string GetTitle()
    {
        if( string.IsNullOrEmpty( this.Title ) )
        {
            return this.Name;
        }
        else
        {
            return this.Title;
        }
    }
}
