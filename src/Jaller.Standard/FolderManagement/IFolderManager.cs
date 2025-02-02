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

namespace Jaller.Standard.FolderManagement;

public interface IFolderManager
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Creates a folder if <see cref="JallerFolder.Id"/> is defaulted,
    /// otherwise modifies an existing folder.
    /// </summary>
    Task ConfigureFolderAsync( JallerFolder folder );

    /// <summary>
    /// Tries to get a folder by the given id.
    /// Returns null if no folder exists.
    /// </summary>
    Task<JallerFolder?> TryGetFolderAsync( int id );

    /// <summary>
    /// Gets the root folder that contains all other folders.
    /// This folder can not be modified or deleted.
    /// </summary>
    Task<RootJallerFolder> GetRootFolderAsync();

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    Task DeleteFolderAsync( int folderId );
}

public static class IFolderManagerExtensions
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    public static Task DeleteFolderAsync( this IFolderManager mgr, JallerFolder folder )
    {
        return mgr.DeleteFolderAsync( folder.Id );
    }
}
