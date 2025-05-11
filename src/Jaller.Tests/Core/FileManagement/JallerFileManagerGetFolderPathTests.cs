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

using Jaller.Core;
using Jaller.Core.Configuration;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Tests.Mocks;

namespace Jaller.Tests.Core.FileManagement;

[TestClass]
public sealed class JallerFileManagerGetFolderPathTests
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

    /// <summary>
    /// If the file does not exist, we get a null list back.
    /// </summary>
    [TestMethod]
    public void CidDoesntExistTest()
    {
        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Files.GetFolderPath(
            "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            MetadataPolicy.Public
        );

        // Check
        Assert.IsNull( folderPath );
    }

    /// <summary>
    /// If the file does not exist, we get a null list back.
    /// </summary>
    [TestMethod]
    public void FileDoesntExistTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "Some File",
            ParentFolder = null,
            MetadataPrivacy = MetadataPolicy.Public
        };

        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Files.GetFolderPath( file, MetadataPolicy.Public );

        // Check
        Assert.IsNull( folderPath );
    }

    /// <summary>
    /// If file is at the root of the file system, ensure an empty list is returned.
    /// </summary>
    [TestMethod]
    public void FileAtRootFolderTest()
    {
        // Setup
        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = null
        };
        this.Core.Files.ConfigureFile( file );

        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Files.GetFolderPath( file, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 0, folderPath.Count );
    }

    /// <summary>
    /// Ensures if only one folder exits, we get a folder path back that only contains it.
    /// </summary>
    [TestMethod]
    public void SingleFolderTest()
    {
        // Setup
        var newFolder = new JallerFolder
        {
            Name = "Test Folder",
            ParentFolder = null,
            MetadataPrivacy = MetadataPolicy.Public
        };

        newFolder = newFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder )
        };

        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = newFolder.Id
        };
        this.Core.Files.ConfigureFile( file );

        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Folders.GetFolderPath( newFolder, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 1, folderPath.Count );

        Assert.AreEqual( newFolder, folderPath[0] );
    }

    [TestMethod]
    public void ThreeFoldersInPathTest()
    {
        // Setup
        var newFolder1 = new JallerFolder
        {
            Name = "Test Folder 1",
            ParentFolder = null,
            MetadataPrivacy = MetadataPolicy.Public
        };

        newFolder1 = newFolder1 with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder1 )
        };

        var newFolder2 = new JallerFolder
        {
            Name = "Test Folder 2",
            ParentFolder = newFolder1.Id,
            MetadataPrivacy = MetadataPolicy.Public
        };

        newFolder2 = newFolder2 with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder2 )
        };

        var newFolder3 = new JallerFolder
        {
            Name = "Test Folder 3",
            ParentFolder = newFolder2.Id,
            MetadataPrivacy = MetadataPolicy.Public
        };

        newFolder3 = newFolder3 with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder3 )
        };

        var file = new JallerFile
        {
            CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
            Name = "file.txt",
            ParentFolder = newFolder3.Id
        };
        this.Core.Files.ConfigureFile( file );

        // Act
        IReadOnlyList<JallerFolder>? folderPath3 = this.Core.Files.GetFolderPath( file, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath3 );
        Assert.AreEqual( 3, folderPath3.Count );
        Assert.AreEqual( newFolder1, folderPath3[0] ); // First slot should be closest to the root.
        Assert.AreEqual( newFolder2, folderPath3[1] );
        Assert.AreEqual( newFolder3, folderPath3[2] );
    }
}
