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
using Jaller.Standard.FolderManagement;
using SethCS.Extensions;

namespace Jaller.Core.Bulk
{
    internal static class JallerFolderExtensions
    {
        // ---------------- Fields ----------------

        internal const string XmlElementName = "folder";

        // ---------------- Methods ----------------

        public static JallerFolder ToJallerFolder( this XElement element, int? parentFolder )
        {
            string name = element.Name.LocalName;
            if( name != XmlElementName )
            {
                throw new ArgumentException( $"Unknown element name, expected {XmlElementName}.", nameof( element ) );
            }

            var folder = new JallerFolder
            {
                Name = "Unknown",
                ParentFolder = parentFolder
            };

            foreach( XElement childElement in element.Elements() )
            {
                string childName = childElement.Name.LocalName;
                if( string.IsNullOrWhiteSpace( childName ) )
                {
                    continue;
                }
                else if( "downloadable".EqualsIgnoreCase( childName ) )
                {
                    folder = folder with
                    {
                        DownloadablePolicy = Enum.Parse<DownloadPolicy>( childElement.Value )
                    };
                }
                else if( "metadata".EqualsIgnoreCase( childName ) )
                {
                    folder = folder with
                    {
                        MetadataPrivacy = Enum.Parse<MetadataPolicy>( childElement.Value )
                    };
                }
                else if( "foldername".EqualsIgnoreCase( childName ) )
                {
                    folder = folder with
                    { 
                        Name = childElement.Value
                    };
                }
            }

            return folder;
        }

        public static XElement ToXml( this JallerFolder folder )
        {
            var element = new XElement( XmlElementName );
            element.Add(
                // Don't care about ID; we'll probably end up making a new ID if the directory is added.
                new XElement( "downloadable", folder.DownloadablePolicy ),
                new XElement( "metadata", folder.MetadataPrivacy ),
                new XElement( "foldername", folder.Name )
                // Don't worry about parent folder, that's determined by the parent XML node.
            );

            return element;
        }
    }
}
