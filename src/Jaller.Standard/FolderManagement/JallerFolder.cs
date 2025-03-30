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

using Jaller.Standard.FileManagement;

namespace Jaller.Standard.FolderManagement;

public sealed record class JallerFolder
{
    // ---------------- Fields ----------------

    public static readonly string DefaultFolderName = "Untitled Folder";

    public static readonly MetadataPolicy DefaultMetadataPrivacy = MetadataPolicy.Private;

    public static readonly DownloadPolicy DefaultDownloadablePolicy = DownloadPolicy.Private;

    // ---------------- Properties ----------------

    public int Id { get; internal init; }

    public required string Name { get; init; }

    /// <summary>
    /// The parent folder's ID.  Null if this is a root folder.
    /// </summary>
    public required int? ParentFolder { get; init; }

    /// <summary>
    /// The publicity of the contents of this folder.
    /// </summary>
    public MetadataPolicy MetadataPrivacy { get; init; } = DefaultMetadataPrivacy;

    /// <summary>
    /// If the contents of this folder are downloadable.
    /// </summary>
    public DownloadPolicy DownloadablePolicy { get; init; } = DefaultDownloadablePolicy;
}
