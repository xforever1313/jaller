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
    /// Creates a new folder of the given name, and returns
    /// the newly created folder.
    /// </summary>
    /// <param name="parentId">
    /// The parent folder.  Null for a root folder.
    /// </param>
    Task<Folder> CreateFolderAsync( string folderName, int? parentId );

    /// <summary>
    /// Changes the folder name.
    /// </summary>
    /// <param name="folderId">
    /// The folder ID to modify.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    Task<Folder> ChangeNameAsync( int folderId, string newName );

    /// <summary>
    /// Changes the metadata privacy policy.
    /// </summary>
    /// <param name="folderId">
    /// The folder ID to modify.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    Task<Folder> ChangeMetadataPrivacyAsync( int folderId, FolderMetadataPrivacy newValue );

    /// <summary>
    /// Changes the download policy of the folder and, by extension, its children.
    /// </summary>
    /// <param name="folder">
    /// The folder to modify.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    Task<Folder> ChangeDownloadPolicyAsync( int folderId, FolderDownloadable newValue );

    /// <summary>
    /// Moves the folder to a different location.
    /// </summary>
    /// <param name="folderId">
    /// The folder to move.
    /// </param>
    /// <param name="parentId">Where to move the folder.  Null for a root folder.</param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    Task<Folder> MoveFolderAsync( int folderId, int? parentId );

    /// <summary>
    /// Deletes this folder *and all children folders and files*.
    /// Do not use unless you really want to napalm everything.
    /// </summary>
    Task DeleteFolderAsync( int folderId );
}

public static class IFolderManagerExtensions
{
    /// <summary>
    /// Creates a new folder of the given name, and returns
    /// the newly created folder.
    /// </summary>
    /// <param name="parent">
    /// The parent folder.  Null for a root folder.
    /// </param>
    public static Task<Folder> CreateFolderAsync( this IFolderManager mgr, string folderName, Folder? parent )
    {
        return mgr.CreateFolderAsync( folderName, parent?.Id );
    }

    /// <summary>
    /// Changes the folder name.
    /// </summary>
    /// <param name="folder">
    /// The folder to modify.  Discard this object
    /// after invoking this method, as it would have changed.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    public static Task<Folder> ChangeNameAsync( this IFolderManager mgr, Folder folder, string newName )
    {
        return mgr.ChangeNameAsync( folder.Id, newName );
    }

    /// <summary>
    /// Changes the metadata privacy policy.
    /// </summary>
    /// <param name="folder">
    /// The folder to modify.  Discard this object
    /// after invoking this method, as it would have changed.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    public static Task<Folder> ChangeMetadataPrivacyAsync( this IFolderManager mgr, Folder folder, FolderMetadataPrivacy newValue )
    {
        return mgr.ChangeMetadataPrivacyAsync( folder.Id, newValue );
    }

    /// <summary>
    /// Changes the download policy of the folder and, by extension, its children.
    /// </summary>
    /// <param name="folder">
    /// The folder to modify.  Discard this object
    /// after invoking this method, as it would have changed.
    /// </param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    public static Task<Folder> ChangeDownloadPolicyAsync( this IFolderManager mgr, Folder folder, FolderDownloadable newValue )
    {
        return mgr.ChangeDownloadPolicyAsync( folder.Id, newValue );
    }

    /// <summary>
    /// Moves the folder to a different location.
    /// </summary>
    /// <param name="folder">
    /// The folder to move.  Discard this object
    /// after invoking this method, as it would have changed.
    /// </param>
    /// <param name="parent">Where to move the folder.  Null for a root folder.</param>
    /// <returns>A new folder object representing the changes made to the folder.</returns>
    public static Task<Folder> MoveFolderAsync( this IFolderManager mgr, Folder folder, Folder? parent )
    {
        return mgr.MoveFolderAsync( folder.Id, parent?.Id );
    }

    /// <summary>
    /// Deletes this folder *and all children folders and files*.
    /// Do not use unless you really want to napalm everything.
    /// </summary>
    public static Task DeleteFolderAsync( this IFolderManager mgr, Folder folder )
    {
        return mgr.DeleteFolderAsync( folder.Id );
    }
}
