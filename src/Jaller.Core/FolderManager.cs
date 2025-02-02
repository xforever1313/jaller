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
using Jaller.Standard;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core
{
    internal sealed class FolderManager : IFolderManager
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;

        private readonly JallerDatabase db;

        // ---------------- Constructor ----------------

        public FolderManager( IJallerCore core, JallerDatabase db )
        {
            this.core = core;
            this.db = db;
        }

        // ---------------- Properties ----------------

        public void ConfigureFolder( JallerFolder folder )
        {
            throw new NotImplementedException();
        }

        public void DeleteFolder( int folderId )
        {
            throw new NotImplementedException();
        }

        public FolderContents GetRootFolder()
        {
            throw new NotImplementedException();
        }

        public FolderContents GetRootFolder( FileMetadataPolicy visibility )
        {
            throw new NotImplementedException();
        }

        public JallerFolder? TryGetFolder( int id )
        {
            JallerDirectory? dir = this.db.Directories.FindById( id );
            if( dir is null )
            {
                return null;
            }

            return new JallerFolder
            {
                Id = dir.Id,
                Name = dir.Name,
                ParentFolder = dir.ParentFolder
            };
        }

        public FolderContents TryGetFolderContents( int folderId, FileMetadataPolicy visibility )
        {
            throw new NotImplementedException();
        }
    }
}
