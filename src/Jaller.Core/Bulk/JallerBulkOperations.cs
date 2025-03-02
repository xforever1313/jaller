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
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core.Bulk
{
    internal sealed class JallerBulkOperations : IBulkOperations
    {
        // ---------------- Fields ----------------

        private const string rootXmlElementName = "JallerFiles";

        private readonly IJallerCore core;

        private const int currentXmlSchemaVersion = 1;

        // ---------------- Constructor ----------------

        public JallerBulkOperations( IJallerCore core )
        {
            this.core = core;
        }

        // ---------------- Methods ----------------

        public void BulkAddMetaData( XDocument doc )
        {
        }

        public XDocument BulkGetAllMetaData( MetadataPolicy policy )
        {
            var doc = new XDocument();

            var dec = new XDeclaration( "1.0", "utf-8", "yes" );
            doc.Add( dec );

            var root = new XElement( rootXmlElementName );
            root.Add( new XAttribute( "version", currentXmlSchemaVersion ) );
            doc.Add( root );

            FolderContents rootContents = this.core.Folders.GetRootFolder( policy );

            return doc;
        }
    }
}
