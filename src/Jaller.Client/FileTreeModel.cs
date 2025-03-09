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

using System.Collections.ObjectModel;
using Jaller.Contracts.FolderManagement;

namespace Jaller.Client;

public class FileTreeModel
{
    // ---------------- Constructor ----------------

    public FileTreeModel( JallerFolderTreeContentsInfo root )
    {
        this.Root = new TreeNode( root );
    }

    // ---------------- Properties ----------------

    public TreeNode Root { get; }

    // ---------------- Methods ----------------

    public bool AddDirectoryInfo( int? parentFolderId, JallerFolderTreeContentsInfo newFolder )
    {
        var queue = new Queue<TreeNode>();
        queue.Enqueue( this.Root );

        while( queue.Any() )
        {
            TreeNode node = queue.Dequeue();
            if( node.FolderContents.FolderId == parentFolderId )
            {
                node.AddChild( newFolder );
                return true;
            }

            foreach( TreeNode childNode in node.ChildFolders.Values )
            {
                queue.Enqueue( childNode );
            }
        }

        return false;
    }
}

public class TreeNode
{
    // ---------------- Fields ----------------

    private readonly Dictionary<int, TreeNode> childFolders;

    // ---------------- Constructor ----------------

    public TreeNode( JallerFolderTreeContentsInfo directory )
    {
        this.FolderContents = directory;
        this.childFolders = new Dictionary<int, TreeNode>();
        this.ChildFolders = new ReadOnlyDictionary<int, TreeNode>( this.childFolders );
    }

    // ---------------- Properties ----------------

    public JallerFolderTreeContentsInfo FolderContents { get; }

    public IReadOnlyDictionary<int, TreeNode> ChildFolders { get; }

    // ---------------- Methods ----------------

    public void AddChild( JallerFolderTreeContentsInfo folder )
    {
        ArgumentNullException.ThrowIfNull( folder.FolderId );

        this.childFolders[folder.FolderId.Value] = new TreeNode( folder ); 
    }
}