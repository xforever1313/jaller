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
}
