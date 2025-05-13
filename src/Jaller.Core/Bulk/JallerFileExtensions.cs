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

namespace Jaller.Core.Bulk;

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

        foreach( XAttribute attribute in element.Attributes() )
        {
            string attrName = attribute.Name.LocalName;
            if( string.IsNullOrEmpty( attrName ) )
            {
                continue;
            }
            else if( "name".EqualsIgnoreCase( attrName ) )
            {
                file = file with
                {
                    Name = attribute.Value
                };
            }
        }

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
            else if( "details".EqualsIgnoreCase( childName ) )
            {
                file = file with
                {
                    Details = childElement.Value
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
            else if( "tags".EqualsIgnoreCase( childName ) )
            {
                var tags = new TagSet();
                foreach( XElement tagElement in childElement.Elements() )
                {
                    string tagElementName = tagElement.Name.LocalName;
                    if( string.IsNullOrWhiteSpace( tagElementName ) )
                    {
                        continue;
                    }
                    else if( "tag".EqualsIgnoreCase( tagElementName ) )
                    {
                        tags.Add( tagElement.Value );
                    }
                }
                file = file with
                { 
                    Tags = tags
                };
            }
            else if( "title".EqualsIgnoreCase( childName ) )
            {
                file = file with 
                {
                    Title = childElement.Value
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
            new XElement( "details", file.Details ),
            new XElement( "downloadable", file.DownloadablePolicy ),
            new XElement( "metadata", file.MetadataPrivacy ),
            new XElement( "mimetype", file.MimeType ),
            new XElement( "title", file.Title ),
            new XAttribute( "name", file.Name )
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
