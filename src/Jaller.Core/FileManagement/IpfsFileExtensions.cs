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

using Jaller.Core.Database;
using Jaller.Standard.FileManagement;

namespace Jaller.Core.FileManagement;

internal static class IpfsFileExtensions
{
    /// <summary>
    /// Transforms the database model to the public API model.
    /// </summary>
    public static JallerFile ToPublicModel( this IpfsFile file )
    {
        return new JallerFile
        {
            CidV1 = file.Cid,
            Description = file.Description,
            DownloadablePolicy = file.DownloadablePolicy,
            MetadataPrivacy = file.MetadataPrivacy,
            MimeType = file.MimeType,
            Name = file.FileName,
            ParentFolder = file.ParentFolder,
            Tags = file.Tags
        };
    }
}
