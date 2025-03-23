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

using System.Diagnostics.CodeAnalysis;
using Jaller.Standard.FolderManagement;

namespace Jaller.Core.FolderManagement
{
    public static class FolderContentsExtensions
    {
        // ---------------- Methods ----------------

        public static bool IsNullOrEmpty( [NotNullWhen( false )] this FolderContents? contents )
        {
            if( contents is null )
            {
                return true;
            }

            bool containsFiles = contents.Files?.Any() ?? false;
            bool containsFolders = contents.ChildFolders?.Any() ?? false;

            return ( containsFiles == false ) && ( containsFolders == false );
        }
    }
}
