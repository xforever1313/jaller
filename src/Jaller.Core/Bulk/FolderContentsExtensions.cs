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
    internal static class FolderContentsExtensions
    {
        // ---------------- Methods ----------------

        /// <summary>
        /// Takes in the given XML Element and adds any files
        /// or directories into the database.
        /// </summary>
        /// <param name="parentFolder">
        /// The parent folder.  Null if root folder.
        /// </param>
        /// <param name="overwriteExistingFiles">
        /// If set to true, if a file already exists in the database with the same CID,
        /// it will be overwritten.  If set to false, the file in the database will
        /// be left alone.
        /// </param>
        /// <returns>
        /// If <paramref name="overwriteExistingFiles"/> was set to true, a list of CID's
        /// that were overridden.  If set to false, a list of CID's that were ignored.
        /// </returns>
        public static BulkAddResult LoadFromXml(
            this XElement parentElement,
            JallerFolder? parentFolder,
            IJallerCore core,
            bool overwriteExistingFiles
        )
        {
            var warnings = new List<string>();
            var errors = new List<string>();

            foreach( XElement element in parentElement.Elements() )
            {
                string childName = element.Name.LocalName;
                if( string.IsNullOrWhiteSpace( childName ) )
                {
                    continue;
                }
                else if( JallerFileExtensions.XmlElementName == childName )
                {
                    JallerFile file = element.ToJallerFile( parentFolder?.Id ?? null );

                    // If the file already exists, only overwrite it if the user wants us to.
                    if( core.Files.FileExists( file.CidV1 ) )
                    {
                        if( overwriteExistingFiles )
                        {
                            core.Files.ConfigureFile( file );
                            warnings.Add( $"File with CID {file.CidV1} already existed, and had its metadata overwritten." );
                        }
                        else
                        {
                            errors.Add( $"File with CID {file.CidV1} already exists in the database, and was not added." );
                        }
                    }
                    else // File does not exist, safe to add in.
                    {
                        core.Files.ConfigureFile( file );
                    }
                }
                else if( JallerFolderExtensions.XmlElementName == childName )
                {
                    JallerFolder folder = element.ToJallerFolder( parentFolder?.Id ?? null );
                    JallerFolder? dbFolder = core.Folders.TryGetFolderByName( folder.ParentFolder, folder.Name );

                    // Folder does not exist, it is safe to add.
                    if( dbFolder is null )
                    {
                        folder = folder with
                        {
                            Id = core.Folders.ConfigureFolder( folder )
                        };

                        BulkAddResult folderResult = LoadFromXml( element, folder, core, overwriteExistingFiles );
                        warnings.AddRange( folderResult.Warnings );
                        errors.AddRange( folderResult.Errors );
                    }
                    else // The folder already exists.
                    {
                        folder = folder with
                        {
                            Id = dbFolder.Id
                        };

                        if( folder.Equals( dbFolder ) )
                        {
                            // The folder from the XML file matches the properties in the DB, we can safely
                            // add to the database since we don't have conflicting folder names.
                            BulkAddResult folderResult = LoadFromXml( element, folder, core, overwriteExistingFiles );
                            warnings.AddRange( folderResult.Warnings );
                            errors.AddRange( folderResult.Errors );
                        }
                        else
                        {
                            // We don't want to clobber an existing folder if it has different visibility properties,
                            // we'll skip importing all of those files for our own sanity.
                            string parentFolderName = parentFolder?.Name ?? "the root directory";
                            errors.Add( $"Unable to add folder {folder.Name}.  A folder already exists within {parentFolder} with the same name, but contains different properties." );
                        }
                    }
                }
            }

            return new BulkAddResult( warnings.AsReadOnly(), errors.AsReadOnly() );
        }

        public static void ToXml(
            this FolderContents folderContents,
            XElement parentElement,
            IJallerCore core,
            MetadataPolicy policy
        )
        {
            foreach( JallerFile file in folderContents.Files ?? Array.Empty<JallerFile>() )
            {
                XElement fileElement = file.ToXml();
                parentElement.Add( fileElement );
            }

            foreach( JallerFolder folder in folderContents.ChildFolders ?? Array.Empty<JallerFolder>() )
            {
                XElement folderElement = folder.ToXml();
                FolderContents? childContents = core.Folders.TryGetFolderContents( folder.Id, policy );

                // I don't love the recursion here, but I also don't know what else to do for
                // walking a tree like this...
                childContents?.ToXml( folderElement, core, policy );
                parentElement.Add( folderElement );
            }
        }

        public static IEnumerable<JallerFile> GetAllFiles(
            this FolderContents folderContents,
            IJallerCore core,
            MetadataPolicy policy
        )
        {
            var list = new List<JallerFile>();

            var queue = new Queue<FolderContents>();
            queue.Enqueue( folderContents );

            while( queue.Any() )
            {
                FolderContents currentContents = queue.Dequeue();
                if( currentContents.Files is not null )
                {
                    list.AddRange( currentContents.Files );
                }

                if( currentContents.ChildFolders is not null )
                {
                    foreach( JallerFolder folder in currentContents.ChildFolders )
                    {
                        FolderContents? childContents = core.Folders.TryGetFolderContents( folder.Id, policy );
                        if( childContents is not null )
                        {
                            queue.Enqueue( childContents );
                        }
                    }
                }
            }

            return list;
        }
    }
}
