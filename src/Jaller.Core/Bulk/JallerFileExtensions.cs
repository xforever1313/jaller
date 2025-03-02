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
using SethCS.Extensions;

namespace Jaller.Core.Bulk
{
    internal static class JallerFileExtensions
    {
        // ---------------- Fields ----------------

        internal const string XmlElementName = "file";

        // ---------------- Methods ----------------

        public static JallerFile ToJallerFile( this XElement element, int? parentFolder )
        {
            string name = element.Name.LocalName;
            if( name != XmlElementName )
            {
                throw new ArgumentException( $"Unknown element name, expected {XmlElementName}.", nameof( element ) );
            }

            XElement? cidElement = element.Elements().FirstOrDefault(
                e => e.Name.LocalName.EqualsIgnoreCase( "cid" )
            );

            if( cidElement is null )
            {
                throw new ArgumentException( "Can not find CID for passed in Jaller File in XML element.", nameof( element ) );
            }

            Cid cid = Cid.Parse( cidElement.Value );

            var file = new JallerFile
            {
                CidV1 = cid.Version1Cid,
                ParentFolder = parentFolder,
                Name = "Untitled"
            };

            foreach( XElement childElement in element.Elements() )
            {
                string childName = childElement.Name.LocalName;
                if( string.IsNullOrWhiteSpace( childName ) )
                {
                    continue;
                }
                else if( "description".EqualsIgnoreCase( childName ) )
                {
                    file = file with
                    {
                        Description = childElement.Value,
                    };
                }
                else if( "downloadable".EqualsIgnoreCase( childName ) )
                {
                    file = file with
                    {
                        DownloadablePolicy = Enum.Parse<DownloadPolicy>( childElement.Value )
                    };
                }
                else if( "metadata".EqualsIgnoreCase( childName ) )
                {
                    file = file with
                    {
                        MetadataPrivacy = Enum.Parse<MetadataPolicy>( childElement.Value )
                    };
                }
                else if( "filename".EqualsIgnoreCase( childName ) )
                {
                    file = file with
                    {
                        Name = childElement.Value
                    };
                }
                else if( "tags".EqualsIgnoreCase( childName ) )
                {
                    var tags = new HashSet<string>();
                    foreach( XElement tagElement in childElement.Elements() )
                    {
                        string tagElementName = tagElement.Name.LocalName;
                        if( string.IsNullOrWhiteSpace( tagElementName ) )
                        {
                            continue;
                        }
                        else if( "tag".EqualsIgnoreCase( tagElementName ) )
                        {
                            tags.Add( tagElementName );
                        }
                    }
                    file = file with
                    { 
                        Tags = tags
                    };
                }
            }

            return file;
        }

        public static XElement ToXml( this JallerFile file )
        {
            var element = new XElement( XmlElementName );
            element.Add(
                new XElement( "cid", file.CidV1 ),
                new XElement( "description", file.Description ),
                new XElement( "downloadable", file.DownloadablePolicy ),
                new XElement( "metadata", file.MetadataPrivacy ),
                new XElement( "mimetype", file.MimeType ),
                new XElement( "filename", file.Name )
                // Ignore directory ID; that's determined by the parent XML node.
            );

            HashSet<string>? tags = file.Tags;
            if( tags is not null )
            {
                var tagListElement = new XElement( "tags" );
                foreach( string tag in tags )
                {
                    var tagElement = new XElement( "tag", tag );
                    tagListElement.Add( tagElement );
                }
                element.Add( tagListElement );
            }

            return element;
        }
    }
}
