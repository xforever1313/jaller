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

namespace Jaller.Core.Database.Models;

public enum FolderMetadataPrivacy : byte
{
    /// <summary>
    /// Folder metadata is inherited from
    /// the parent folder.
    /// </summary>
    Inherited = 0,
    
    /// <summary>
    /// Metadata is only viewable to logged-in users.
    /// </summary>
    Private = 100,
    
    /// <summary>
    /// The metadata is only viewable in machine-readable
    /// format (e.g. XML or RSS), but will not appear
    /// on the human-facing front-end.
    /// </summary>
    MachinePublic = 200,
    
    /// <summary>
    /// Metadata can be viewed by anyone, even if they
    /// are not logged in.  This does not mean
    /// that the file can be downloaded from the gateway,
    /// however.
    /// </summary>
    Public = 255
}

public enum FolderDownloadable : byte
{
    /// <summary>
    /// Folder downloadable flag is inherited from
    /// the parent folder.
    /// </summary>
    Inherited = 0,
    
    /// <summary>
    /// Files within the folder can only be downloaded
    /// by logged-in users.
    /// </summary>
    Private = 100,
    
    /// <summary>
    /// The files within the folder can not be downloaded from the gateway
    /// even if a user is logged in.
    /// Only metadata can be viewed.
    /// </summary>
    NoDownload = 200,
    
    /// <summary>
    /// The files within the folder can be downloaded by anyone, even if they
    /// are not logged in.  Do *not* set a folder to this
    /// if you do not have permission to distribute the files within the folder.
    /// </summary>
    Public = 255
}

public enum FileMetadataPrivacy : byte
{
    /// <summary>
    /// File metadata is inherited from
    /// the parent folder.
    /// </summary>
    Inherited = 0,
    
    /// <summary>
    /// File metadata is only viewable to logged-in users.
    /// </summary>
    Private = 100,
    
    /// <summary>
    /// The metadata is only viewable in machine-readable
    /// format (e.g. XML or RSS), but will not appear
    /// on the human-facing front-end.
    /// </summary>
    MachinePublic = 200,
    
    /// <summary>
    /// Metadata can be viewed by anyone, even if they
    /// are not logged in.  This does not mean
    /// that the file can be downloaded from the gateway,
    /// however.
    /// </summary>
    Public = 255
}

public enum FileDownloadable : byte
{
    /// <summary>
    /// File downloadable flag is inherited from
    /// the parent folder.
    /// </summary>
    Inherited = 0,
    
    /// <summary>
    /// File can only be downloaded
    /// by logged-in users.
    /// </summary>
    Private = 100,
    
    /// <summary>
    /// File can not be downloaded from the gateway
    /// even if a user is logged in.
    /// Only metadata can be viewed.
    /// </summary>
    NoDownload = 200,
    
    /// <summary>
    /// The file can be downloaded by anyone, even if they
    /// are not logged in.  Do *not* set a file to this
    /// if you do not have permission to distribute the file.
    /// </summary>
    Public = 255
}
