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

using System.Globalization;
using System.Xml.Linq;
using CsvHelper;
using Jaller.Core.FileManagement;
using Jaller.Standard;
using Jaller.Standard.Bulk;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core.Bulk
{
    internal sealed class JallerBulkOperations : IJallerBulkOperations
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

        public BulkAddResult BulkAddMetaData( XDocument doc, bool overwriteExistingFiles )
        {
            XElement? root = doc.Root;
            if( root is null )
            {
                throw new ArgumentException(
                    "Passed in XML document contains no root element.",
                    nameof( doc )
                );
            }

            if( rootXmlElementName != root.Name.LocalName )
            {
                throw new ArgumentException(
                    "Got a root XML node that was unexpected.  Is this the right type of XML file?",
                    nameof( doc )
                );
            }

            BulkAddResult result = root.LoadFromXml( null, this.core, overwriteExistingFiles );
            return result;
        }

        public XDocument BulkGetAllMetaData( MetadataPolicy policy )
        {
            var dec = new XDeclaration( "1.0", "utf-8", "yes" );
            var doc = new XDocument( dec );

            var root = new XElement( rootXmlElementName );
            root.Add( new XAttribute( "version", currentXmlSchemaVersion ) );
            doc.Add( root );

            FolderContents rootContents = this.core.Folders.GetRootFolder( policy );

            rootContents.ToXml( root, this.core, policy );

            return doc;
        }

        public string GetAllFileMetadataAsCsv( MetadataPolicy policy )
        {
            FolderContents rootContents = this.core.Folders.GetRootFolder( policy );

            var csvModels = new List<CsvModel>();

            foreach( JallerFile file in rootContents.GetAllFiles( this.core, policy ) )
            {
                csvModels.Add( new CsvModel( file ) );
            }

            using var writer = new StringWriter();
            using var csv = new CsvWriter( writer, CultureInfo.InvariantCulture );
            csv.WriteRecords( csvModels );

            return writer.ToString();
        }

        private sealed class CsvModel
        {
            // ---------------- Constructor ----------------

            public CsvModel( JallerFile file )
            {
                Cid cid = Cid.Parse( file.CidV1 );
                this.CidV0 = cid.Version0Cid;
                this.CidV1 = cid.Version1Cid;

                this.FileName = file.Name;
                this.MimeType = file.GetMimeType();
            }

            // ---------------- Properties ----------------

            public string FileName { get; }

            public string CidV0 { get; }

            public string CidV1 { get; }

            public string MimeType { get; }
        }
    }
}
