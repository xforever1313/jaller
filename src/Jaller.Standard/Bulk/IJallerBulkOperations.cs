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

using System.Xml.Linq;
using Jaller.Standard.FileManagement;

namespace Jaller.Standard.Bulk
{
    public interface IJallerBulkOperations
    {
        /// <summary>
        /// Bluk adds metadata for files using an XML format.
        /// </summary>
        /// <returns>
        /// The result of adding the metadata, including warning and error messages.
        /// </returns>
        BulkAddResult BulkAddMetaData( XDocument doc, bool overwriteExistingFiles );

        /// <summary>
        /// Creates an XML file that contains all files in an XML format.
        /// </summary>
        XDocument BulkGetAllMetaData( MetadataPolicy policy );
    }
}
