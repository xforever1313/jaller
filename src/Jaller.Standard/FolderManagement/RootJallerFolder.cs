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
/// The root folder that contains all of Jaller's folders and files.
/// </summary>
public sealed record class RootJallerFolder
{
    /// <summary>
    /// List of folders that live at the root-level of the Jaller file
    /// system.
    /// 
    /// Null for no directories.
    /// </summary>
    public required IReadOnlyList<JallerFolder>? ChildFolders { get; init; }

    /// <summary>
    /// Files that live at the root level of the Jaller file system.
    /// Null for no files.
    /// </summary>
    public required IReadOnlyList<JallerFile>? Files { get; init; }
}
