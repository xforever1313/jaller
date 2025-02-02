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

/// <summary>
/// Contents of a folder.
/// </summary>
public sealed record class FolderContents
{
    /// <summary>
    /// Child folders of the <see cref="JallerFolder"/>.
    /// Set to null if there are no child folders.
    /// </summary>
    public required IReadOnlyList<JallerFolder>? ChildFolders { get; init; }

    /// <summary>
    /// Files that exist in the folder.
    /// 
    /// Set to null if there are no files within the folder.
    /// </summary>
    public required IReadOnlyList<JallerFile>? Files { get; init; }
}
