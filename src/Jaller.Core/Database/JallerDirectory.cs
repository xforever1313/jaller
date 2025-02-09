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
using LiteDB;

namespace Jaller.Core.Database;

internal sealed record class JallerDirectory
{
    [BsonId( true )]
    public int Id { get; set; }

    public string Name { get; init; } = "Untitled Folder";

    public int? ParentFolder { get; init; } = null;

    /// <summary>
    /// The publicity of the contents of this folder.
    /// </summary>
    public MetadataPolicy MetadataPrivacy { get; init; } = MetadataPolicy.Private;

    /// <summary>
    /// If the contents of this folder are downloadable.
    /// </summary>
    public DownloadPolicy DownloadablePolicy { get; init; } = DownloadPolicy.Private;

    public List<int>? ChildrenFolders { get; init; } = null;

    /// <summary>
    /// CID of files that this folder contains.
    /// Null for no files.
    /// </summary>
    public List<string>? Files { get; init; } = null;
}
