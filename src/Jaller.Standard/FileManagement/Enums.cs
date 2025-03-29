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

/// <summary>
/// Is the metadata of a file viewable?
/// </summary>
/// <remarks>
/// Must be int instead of byte or a casting exception happens with LiteDB.
/// </remarks>
public enum MetadataPolicy : int
{
    /// <summary>
    /// Metadata is only viewable to logged-in users.
    /// 
    /// This is the default.
    /// </summary>
    Private = 0,

    /// <summary>
    /// Metadata can be viewed by anyone, even if they
    /// are not logged in.  This does not mean
    /// that the file can be downloaded from the gateway,
    /// however.
    /// </summary>
    Public = 255
}

/// <summary>
/// Is the file or folder contents downloadable from this gateway?
/// </summary>
/// <remarks>
/// Must be int instead of byte or a casting exception happens with LiteDB.
/// </remarks>
public enum DownloadPolicy : int
{
    /// <summary>
    /// File or folder contents can only be downloaded
    /// by logged-in users.
    /// 
    /// This is the default.
    /// </summary>
    Private = 0,

    /// <summary>
    /// The file or folder contents can not be downloaded from the gateway
    /// even if a user is logged in.
    /// Only metadata can be viewed.
    /// </summary>
    NoDownload = 200,

    /// <summary>
    /// The file or folder contents can be downloaded by anyone, even if they
    /// are not logged in.  Do *not* set a folder to this
    /// if you do not have permission to distribute the files within the folder.
    /// </summary>
    Public = 255
}
