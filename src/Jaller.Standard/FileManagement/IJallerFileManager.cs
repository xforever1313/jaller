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

using Jaller.Standard.FolderManagement;

namespace Jaller.Standard.FileManagement;

public interface IJallerFileManager
{
    /// <summary>
    /// Returns the total number of files that have been created.
    /// </summary>
    int GetFileCount();

    /// <summary>
    /// Creates a file if <see cref="JallerFile.CidV1"/> does not exist
    /// in the database, otherwise modifies an existing file.
    /// </summary>
    void ConfigureFile( JallerFile file );

    /// <summary>
    /// Tries to get a file by the given CID.
    /// Returns null if no file exists.
    /// </summary>
    JallerFile? TryGetFile( string fileCid );

    /// <summary>
    /// Deletes the file and its information.
    /// </summary>
    void DeleteFile( string fileCid );

    /// <summary>
    /// Gets the path of folders to the given file's CID.
    /// </summary>
    /// <remarks>
    /// An exception is thrown if while building the path, a folder is found
    /// between the passed in folder and the root no longer exists.
    /// </remarks>
    /// <param name="fileCid">The file id to get the path of.</param>
    /// <returns>
    /// A list of folders that represent the path to the given file.
    /// The 0th index in the list represents the folder closest to the root folder.
    /// The last element in the list is the folder that contains the CID.
    ///
    /// Returns an empty list if the file is in the root file.
    /// 
    /// Null if the given file is not found.
    /// </returns>
    IReadOnlyList<JallerFolder>? GetFolderPath( string fileCid, MetadataPolicy visibility );
}

public static class IJallerFileManagerExtensions
{
    /// <summary>
    /// Returns true if a file of the given CID exists.
    /// </summary>
    public static bool FileExists( this IJallerFileManager files, string cid )
    {
        return files.TryGetFile( cid ) is not null;
    }

    /// <summary>
    /// Gets the path of folders to the given file's CID.
    /// </summary>
    /// <remarks>
    /// An exception is thrown if while building the path, a folder is found
    /// between the passed in folder and the root no longer exists.
    /// </remarks>
    /// <param name="file">The file to get the path of.</param>
    /// <returns>
    /// A list of folders that represent the path to the given file.
    /// The 0th index in the list represents the folder closest to the root folder.
    /// The last element in the list is the folder that contains the CID.
    ///
    /// Returns an empty list if the file is in the root file.
    /// 
    /// Null if the given file is not found.
    /// </returns>
    public static IReadOnlyList<JallerFolder>? GetFolderPath( this IJallerFileManager files, JallerFile file, MetadataPolicy visibility )
    {
        return files.GetFolderPath( file.CidV1, visibility );
    }
}
