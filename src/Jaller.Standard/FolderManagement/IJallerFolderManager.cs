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

public interface IJallerFolderManager
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Returns the total number of folders that have been created.
    /// </summary>
    int GetFolderCount();

    /// <summary>
    /// Creates a folder if <see cref="JallerFolder.Id"/> is defaulted,
    /// otherwise modifies an existing folder.
    /// </summary>
    /// <returns>
    /// The ID of the folder.  This will be a new ID if the folder was created,
    /// otherwise it should be the same ID as the passed in folder.
    /// </returns>
    int ConfigureFolder( JallerFolder folder );

    /// <summary>
    /// Tries to get all of the contents within a folder.
    /// </summary>
    /// <returns>
    /// Null if the passed in folder does not exist.
    /// </returns>
    FolderContents? TryGetFolderContents( int folderId, MetadataPolicy visibility );

    /// <summary>
    /// Tries to get a folder by the given id.
    /// Returns null if no folder exists.
    /// </summary>
    JallerFolder? TryGetFolder( int id );

    /// <summary>
    /// Tries to find a folder by looking in the database and finding
    /// the folder that matches the folder based on the given parent folder ID and
    /// the folder name.
    /// </summary>
    /// <returns>
    /// The folder as it exists in the database, otherwise null if no
    /// folder was found.
    /// </returns>
    JallerFolder? TryGetFolderByName( int? parentFolderId, string folderName );

    /// <summary>
    /// Gets the root folder that contains all other folders.
    /// This folder can not be modified or deleted.
    /// </summary>
    FolderContents GetRootFolder( MetadataPolicy visibility );

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    void DeleteFolder( int folderId );

    /// <summary>
    /// Gets the path of folders to the given folder ID.
    /// </summary>
    /// <remarks>
    /// An exception is thrown if while building the path, a folder is found
    /// between the passed in folder and the root no longer exists.
    /// </remarks>
    /// <param name="folderId">The folder id to get the path of.</param>
    /// <returns>
    /// A list of folders that represent the path to the given folder.
    /// The 0th index in the list represents the folder closest to the root folder.
    /// The last element in the list is the passed in folder.
    ///
    /// Returns an empty list if null is passed into represent the root folder.
    /// 
    /// Null if the given folder is not found.
    /// </returns>
    IReadOnlyList<JallerFolder>? TryGetFolderPath( int? folderId, MetadataPolicy visibility );
}

public static class IFolderManagerExtensions
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Tries to get all of the contents within a folder.
    /// </summary>
    public static FolderContents? TryGetFolderContents( this IJallerFolderManager mgr, JallerFolder folder, MetadataPolicy visibility )
    {
        return mgr.TryGetFolderContents( folder.Id, visibility );
    }

    /// <summary>
    /// Deletes the given folder.
    /// </summary>
    public static void DeleteFolder( this IJallerFolderManager mgr, JallerFolder folder )
    {
        mgr.DeleteFolder( folder.Id );
    }

    /// <summary>
    /// Gets the path of folders to the given folder.
    /// </summary>
    /// <remarks>
    /// An exception is thrown if while building the path, a folder is found
    /// between the passed in folder and the root no longer exists.
    /// </remarks>
    /// <param name="folderId">The folder id to get the path of.</param>
    /// <returns>
    /// A list of folders that represent the path to the given folder.
    /// The 0th index in the list represents the folder closest to the root folder.
    /// The last element in the list is the passed in folder.
    ///
    /// Returns an empty list if null is passed into represent the root folder.
    /// 
    /// Null if the given folder is not found.
    /// </returns>
    public static IReadOnlyList<JallerFolder>? TryGetFolderPath( this IJallerFolderManager mgr, JallerFolder? folder, MetadataPolicy visiblity )
    {
        return mgr.TryGetFolderPath( folder?.Id, visiblity );
    }
}
