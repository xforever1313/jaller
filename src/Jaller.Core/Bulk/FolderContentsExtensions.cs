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
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core.Bulk
{
    internal static class FolderContentsExtensions
    {
        // ---------------- Methods ----------------

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
            }
        }
    }
}
