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

public interface IFolderManager
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Creates a folder if <see cref="JallerFolder.Id"/> is defaulted,
    /// otherwise modifies an existing folder.
    /// </summary>
    void ConfigureFolder( JallerFolder folder );

    /// <summary>
    /// Tries to get all of the contents within a folder.
    /// </summary>
    /// <returns>
    /// Null if the passed in folder does not exist.
    /// </returns>
    FolderContents? TryGetFolderContents( int folderId, FileMetadataPolicy visibility );

    /// <summary>
    /// Tries to get a folder by the given id.
    /// Returns null if no folder exists.
    /// </summary>
    JallerFolder? TryGetFolder( int id );

    /// <summary>
    /// Gets the root folder that contains all other folders.
    /// This folder can not be modified or deleted.
    /// </summary>
    FolderContents GetRootFolder( FileMetadataPolicy visibility );

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    void DeleteFolder( int folderId );
}

public static class IFolderManagerExtensions
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Tries to get all of the contents within a folder.
    /// </summary>
    public static FolderContents? TryGetFolderContents( this IFolderManager mgr, JallerFolder folder, FileMetadataPolicy visibility )
    {
        return mgr.TryGetFolderContents( folder.Id, visibility );
    }

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    public static void DeleteFolder( this IFolderManager mgr, JallerFolder folder )
    {
        mgr.DeleteFolder( folder.Id );
    }
}
