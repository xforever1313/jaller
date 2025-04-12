//
// Jaller - An advanced IPFS Gateway
// Copyright (C) 2025 Seth Hendrick
// m
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

using Jaller.Core;
using Jaller.Core.Configuration;
using Jaller.Core.Exceptions;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Tests.Mocks;

namespace Jaller.Tests.Core.FileManagement;

[TestClass]
public sealed class JallerFileManagerTests
{
    // ---------------- Fields ----------------

    private JallerConfig? config;

    private JallerCore? core;

    // ---------------- Setup / Teardown ----------------

    [TestInitialize]
    public void TestSetup()
    {
        this.config = new JallerConfig();
        this.config.UseInMemoryDatabase();

        this.core = new JallerCore( this.config, new StubLogger() );
        this.core.Init();
    }

    [TestCleanup]
    public void TestTeardown()
    {
        this.core?.Dispose();
    }

    // ---------------- Properties ----------------

    public JallerConfig Config
    {
        get
        {
            Assert.IsNotNull( this.config );
            return this.config;
        }
    }

    public JallerCore Core
    {
        get
        {
            Assert.IsNotNull( this.core );
            return this.core;
        }
    }

    // ---------------- Tests ----------------

    [TestMethod]
    public void DeleteFileThatDoesNotExistTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };

        // Act
        JallerFile? foundFile = this.Core.Files.TryGetFile( file.CidV1 );
        this.Core.Files.DeleteFile( file.CidV1 );

        // Check
        Assert.IsNull( foundFile );
        Assert.IsFalse( this.Core.Files.FileExists( file.CidV1 ) );
        Assert.AreEqual( 0, this.Core.Files.GetFileCount() );
    }

    [TestMethod]
    public void ParentFolderDoesNotExistTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "Does not exist.txt",
            ParentFolder = 124
        };

        // Act
        Assert.ThrowsException<FolderNotFoundException>( () => this.Core.Files.ConfigureFile( file ) );
        int fileCount = this.Core.Files.GetFileCount();
        JallerFile? actualFile = this.Core.Files.TryGetFile( file.CidV1 );

        // Check
        Assert.AreEqual( 0, fileCount );
        Assert.IsFalse( this.Core.Files.FileExists( file.CidV1 ) );
        Assert.IsNull( actualFile );
    }

    [TestMethod]
    public void CreateFileAtRootDirectoryTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };

        // Act
        this.Core.Files.ConfigureFile( file );

        int totalFiles = this.Core.Files.GetFileCount();
        JallerFile? actualFile = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents rootContents = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );

        // Check
        Assert.AreEqual( 1, totalFiles );
        Assert.IsNotNull( actualFile );
        Assert.AreEqual( file, actualFile );
        Assert.IsTrue( this.Core.Files.FileExists( file.CidV1 ) );

        Assert.IsNull( rootContents.ChildFolders );
        Assert.IsNotNull( rootContents.Files );
        Assert.AreEqual( 1, rootContents.Files.Count() );
        Assert.AreEqual( file, rootContents.Files.First() );
    }

    [TestMethod]
    public void CreateFileAtRootDirectoryWithPublicSettingsTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null,
            DownloadablePolicy = DownloadPolicy.Public,
            MetadataPrivacy = MetadataPolicy.Public,
            Description = "Some Description",
            MimeType = "application/text"
        };

        // Act
        this.Core.Files.ConfigureFile( file );

        int totalFiles = this.Core.Files.GetFileCount();
        JallerFile? actualFile = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents rootContents = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );

        // Check
        Assert.AreEqual( 1, totalFiles );
        Assert.IsNotNull( actualFile );
        Assert.AreEqual( file, actualFile );
        Assert.IsTrue( this.Core.Files.FileExists( file.CidV1 ) );

        Assert.IsNull( rootContents.ChildFolders );
        Assert.IsNotNull( rootContents.Files );
        Assert.AreEqual( 1, rootContents.Files.Count() );
        Assert.AreEqual( file, rootContents.Files.First() );
    }

    [TestMethod]
    public void CreateFileInDirectoryTest()
    {
        // Setup
        var newFolder = new JallerFolder
        {
            Name = "Test Folder",
            ParentFolder = null
        };

        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };

        // Act
        int newFolderId = this.Core.Folders.ConfigureFolder( newFolder );
        Assert.AreNotEqual( 0, newFolderId );

        file = file with
        { 
            ParentFolder = newFolderId
        };

        this.Core.Files.ConfigureFile( file );

        int totalFiles = this.Core.Files.GetFileCount();
        JallerFile? actualFile = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents? folderContents = this.Core.Folders.TryGetFolderContents( newFolderId, MetadataPolicy.Private );

        // Check
        Assert.AreEqual( 1, totalFiles );
        Assert.IsNotNull( actualFile );
        Assert.AreEqual( file, actualFile );
        Assert.IsTrue( this.Core.Files.FileExists( file.CidV1 ) );

        Assert.IsNotNull( folderContents );
        Assert.IsNull( folderContents.ChildFolders );
        Assert.IsNotNull( folderContents.Files );
        Assert.AreEqual( 1, folderContents.Files.Count() );
        Assert.AreEqual( file, folderContents.Files.First() );
    }


    [TestMethod]
    public void MoveFileIntoDifferentFolderTest()
    {
        // Setup

        // Setup root
        var rootFolder = new JallerFolder
        {
            Name = "root",
            ParentFolder = null
        };

        rootFolder = rootFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( rootFolder )
        };
        Assert.AreNotEqual( 0, rootFolder.Id );

        // Setup children
        var child1 = new JallerFolder
        {
            Name = "Child 1",
            ParentFolder = rootFolder.Id
        };

        child1 = child1 with
        {
            Id = this.Core.Folders.ConfigureFolder( child1 )
        };
        Assert.AreNotEqual( 0, child1.Id );

        // Setup File
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = rootFolder.Id
        };

        // Act
        this.Core.Files.ConfigureFile( file );

        int totalFilesBeforeMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileBeforeMove = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents? rootFolderContentsBeforeMove = this.Core.Folders.TryGetFolderContents( rootFolder.Id, MetadataPolicy.Private );
        FolderContents? childFolderContentsBeforeMove = this.Core.Folders.TryGetFolderContents( child1.Id, MetadataPolicy.Private );

        // Perform move
        JallerFile fileAfterMove = file with
        {
            ParentFolder = child1.Id
        };

        this.Core.Files.ConfigureFile( fileAfterMove );

        int totalFilesAfterMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileAfterMove = this.Core.Files.TryGetFile( fileAfterMove.CidV1 );
        FolderContents? rootFolderContentsAfterMove = this.Core.Folders.TryGetFolderContents( rootFolder.Id, MetadataPolicy.Private );
        FolderContents? childFolderContentsAfterMove = this.Core.Folders.TryGetFolderContents( child1.Id, MetadataPolicy.Private );

        // Check

        // Check before move.
        Assert.AreEqual( 1, totalFilesBeforeMove );
        Assert.IsNotNull( actualFileBeforeMove );
        Assert.AreEqual( file, actualFileBeforeMove );

        Assert.IsNotNull( rootFolderContentsBeforeMove );
        Assert.IsNotNull( rootFolderContentsBeforeMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsBeforeMove.ChildFolders.Count );
        Assert.AreEqual( child1, rootFolderContentsBeforeMove.ChildFolders.First() );
        Assert.IsNotNull( rootFolderContentsBeforeMove.Files );
        Assert.AreEqual( 1, rootFolderContentsBeforeMove.Files.Count() );
        Assert.AreEqual( file, rootFolderContentsBeforeMove.Files.First() );

        Assert.IsNotNull( childFolderContentsBeforeMove );
        Assert.IsNull( childFolderContentsBeforeMove.ChildFolders );
        Assert.IsNull( childFolderContentsBeforeMove.Files );

        // Check after move.
        Assert.AreEqual( 1, totalFilesAfterMove );
        Assert.IsNotNull( actualFileAfterMove );
        Assert.AreEqual( fileAfterMove, actualFileAfterMove );

        Assert.IsNotNull( rootFolderContentsAfterMove );
        Assert.IsNotNull( rootFolderContentsAfterMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsAfterMove.ChildFolders.Count );
        Assert.AreEqual( child1, rootFolderContentsAfterMove.ChildFolders.First() );
        Assert.IsNull( rootFolderContentsAfterMove.Files );

        Assert.IsNotNull( childFolderContentsAfterMove );
        Assert.IsNull( childFolderContentsAfterMove.ChildFolders );
        Assert.IsNotNull( childFolderContentsAfterMove.Files );
        Assert.AreEqual( 1, childFolderContentsAfterMove.Files.Count() );
        Assert.AreEqual( fileAfterMove, childFolderContentsAfterMove.Files.First() );
    }

    [TestMethod]
    public void MoveFileIntoRootFolderTest()
    {
        // Setup

        // Setup root
        var childFolder = new JallerFolder
        {
            Name = "root",
            ParentFolder = null
        };

        childFolder = childFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( childFolder )
        };
        Assert.AreNotEqual( 0, childFolder.Id );

        // Setup Folder
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = childFolder.Id
        };

        // Act
        this.Core.Files.ConfigureFile( file );

        int totalFilesBeforeMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileBeforeMove = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents rootFolderContentsBeforeMove = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );
        FolderContents? childFolderContentsBeforeMove = this.Core.Folders.TryGetFolderContents( childFolder.Id, MetadataPolicy.Private );

        // Perform move
        JallerFile fileAfterMove = file with
        {
            ParentFolder = null
        };

        this.Core.Files.ConfigureFile( fileAfterMove );

        int totalFilesAfterMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileAfterMove = this.Core.Files.TryGetFile( fileAfterMove.CidV1 );
        FolderContents rootFolderContentsAfterMove = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );
        FolderContents? childFolderContentsAfterMove = this.Core.Folders.TryGetFolderContents( childFolder.Id, MetadataPolicy.Private );

        // Check

        // Check before move.
        Assert.AreEqual( 1, totalFilesBeforeMove );
        Assert.IsNotNull( actualFileBeforeMove );
        Assert.AreEqual( file, actualFileBeforeMove );

        Assert.IsNotNull( rootFolderContentsBeforeMove );
        Assert.IsNotNull( rootFolderContentsBeforeMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsBeforeMove.ChildFolders.Count );
        Assert.AreEqual( childFolder, rootFolderContentsBeforeMove.ChildFolders.First() );
        Assert.IsNull( rootFolderContentsBeforeMove.Files );

        Assert.IsNotNull( childFolderContentsBeforeMove );
        Assert.IsNull( childFolderContentsBeforeMove.ChildFolders );
        Assert.IsNotNull( childFolderContentsBeforeMove.Files );
        Assert.AreEqual( 1, childFolderContentsBeforeMove.Files.Count() );
        Assert.AreEqual( file, childFolderContentsBeforeMove.Files.First() );

        // Check after move.
        Assert.AreEqual( 1, totalFilesAfterMove );
        Assert.IsNotNull( actualFileAfterMove );
        Assert.AreEqual( fileAfterMove, actualFileAfterMove );

        Assert.IsNotNull( rootFolderContentsAfterMove );
        Assert.IsNotNull( rootFolderContentsAfterMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsAfterMove.ChildFolders.Count );
        Assert.AreEqual( childFolder, rootFolderContentsAfterMove.ChildFolders.First() );
        Assert.IsNotNull( rootFolderContentsAfterMove.Files );
        Assert.AreEqual( 1, rootFolderContentsAfterMove.Files.Count );

        Assert.IsNotNull( childFolderContentsAfterMove );
        Assert.IsNull( childFolderContentsAfterMove.ChildFolders );
        Assert.IsNull( childFolderContentsAfterMove.Files );
    }

    [TestMethod]
    public void MoveOutOfRootFolderTest()
    {
        // Setup

        // Setup Folder
        var childFolder = new JallerFolder
        {
            Name = "root",
            ParentFolder = null
        };

        childFolder = childFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( childFolder )
        };
        Assert.AreNotEqual( 0, childFolder.Id );

        // Setup File
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };

        // Act
        this.Core.Files.ConfigureFile( file );

        int totalFilesBeforeMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileBeforeMove = this.Core.Files.TryGetFile( file.CidV1 );
        FolderContents rootFolderContentsBeforeMove = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );
        FolderContents? childFolderContentsBeforeMove = this.Core.Folders.TryGetFolderContents( childFolder.Id, MetadataPolicy.Private );

        // Perform move
        JallerFile fileAfterMove = file with
        {
            ParentFolder = childFolder.Id
        };

        this.Core.Files.ConfigureFile( fileAfterMove );

        int totalFilesAfterMove = this.Core.Files.GetFileCount();
        JallerFile? actualFileAfterMove = this.Core.Files.TryGetFile( fileAfterMove.CidV1 );
        FolderContents rootFolderContentsAfterMove = this.Core.Folders.GetRootFolder( MetadataPolicy.Private );
        FolderContents? childFolderContentsAfterMove = this.Core.Folders.TryGetFolderContents( childFolder.Id, MetadataPolicy.Private );

        // Check

        // Check before move.
        Assert.AreEqual( 1, totalFilesBeforeMove );
        Assert.IsNotNull( actualFileBeforeMove );
        Assert.AreEqual( file, actualFileBeforeMove );

        Assert.IsNotNull( rootFolderContentsBeforeMove );
        Assert.IsNotNull( rootFolderContentsBeforeMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsBeforeMove.ChildFolders.Count );
        Assert.AreEqual( childFolder, rootFolderContentsBeforeMove.ChildFolders.First() );
        Assert.IsNotNull( rootFolderContentsBeforeMove.Files );
        Assert.AreEqual( 1, rootFolderContentsBeforeMove.Files.Count );
        Assert.AreEqual( file, rootFolderContentsBeforeMove.Files.First() );

        Assert.IsNotNull( childFolderContentsBeforeMove );
        Assert.IsNull( childFolderContentsBeforeMove.ChildFolders );
        Assert.IsNull( childFolderContentsBeforeMove.Files );

        // Check after move.
        Assert.AreEqual( 1, totalFilesAfterMove );
        Assert.IsNotNull( actualFileAfterMove );
        Assert.AreEqual( fileAfterMove, actualFileAfterMove );

        Assert.IsNotNull( rootFolderContentsAfterMove );
        Assert.IsNotNull( rootFolderContentsAfterMove.ChildFolders );
        Assert.AreEqual( 1, rootFolderContentsAfterMove.ChildFolders.Count );
        Assert.AreEqual( childFolder, rootFolderContentsAfterMove.ChildFolders.First() );
        Assert.IsNull( rootFolderContentsAfterMove.Files );

        Assert.IsNotNull( childFolderContentsAfterMove );
        Assert.IsNull( childFolderContentsAfterMove.ChildFolders );
        Assert.IsNotNull( childFolderContentsAfterMove.Files );
        Assert.AreEqual( 1, childFolderContentsAfterMove.Files.Count );
        Assert.AreEqual( fileAfterMove, childFolderContentsAfterMove.Files.First() );
    }

    [TestMethod]
    public void DeleteFileFromRootTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };

        // Act
        this.Core.Files.ConfigureFile( file );
        Assert.AreEqual( 1, this.Core.Files.GetFileCount() );

        this.Core.Files.DeleteFile( file.CidV1 );

        // Check
        Assert.AreEqual( 0, this.Core.Files.GetFileCount() );
        Assert.IsNull( this.Core.Files.TryGetFile( file.CidV1 ) );
    }

    [TestMethod]
    public void DeleteFileFromFolderTest()
    {
        // Setup
        var newFolder = new JallerFolder
        {
            Name = "Test Folder",
            ParentFolder = null
        };

        newFolder = newFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder )
        };
        Assert.AreNotEqual( 0, newFolder.Id );

        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = newFolder.Id
        };

        // Act
        this.Core.Files.ConfigureFile( file );
        Assert.AreEqual( 1, this.Core.Files.GetFileCount() );

        this.Core.Files.DeleteFile( file.CidV1 );
        FolderContents? folderContents = this.Core.Folders.TryGetFolderContents( newFolder.Id, MetadataPolicy.Private );

        // Check
        Assert.AreEqual( 0, this.Core.Files.GetFileCount() );
        Assert.IsNull( this.Core.Files.TryGetFile( file.CidV1 ) );
        
        Assert.IsNotNull( folderContents );
        Assert.IsNull( folderContents.ChildFolders );
        Assert.IsNull( folderContents.Files );
    }

    [TestMethod]
    public void FolderDeletionDeletesFilesTest()
    {
        // Setup
        // Setup folder
        var folder = new JallerFolder
        {
            Name = "root",
            ParentFolder = null
        };

        folder = folder with
        {
            Id = this.Core.Folders.ConfigureFolder( folder )
        };
        Assert.AreNotEqual( 0, folder.Id );

        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = folder.Id
        };

        // Act
        this.Core.Files.ConfigureFile( file );
        Assert.AreEqual( 1, this.Core.Folders.GetFolderCount() );
        Assert.AreEqual( 1, this.Core.Files.GetFileCount() );

        this.Core.Folders.DeleteFolder( folder.Id );

        // Check
        Assert.AreEqual( 0, this.Core.Folders.GetFolderCount() );
        Assert.AreEqual( 0, this.Core.Files.GetFileCount() );
    }
}
