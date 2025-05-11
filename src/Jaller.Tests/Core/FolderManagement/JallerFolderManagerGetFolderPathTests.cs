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

namespace Jaller.Tests.Core.FolderManagement;

[TestClass]
public sealed class JallerFolderManagerGetFolderPathTests
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
    /// Ensures if the root folder ID is passed in, an empty list is returned.
    /// </summary>
    [TestMethod]
    public void NullRootFolderIdTest()
    {
        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Folders.GetFolderPath( null, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 0, folderPath.Count );
    }

    /// <summary>
    /// Ensures if the root folder ID is passed in, an empty list is returned.
    /// </summary>
    [TestMethod]
    public void NullRootFolderTest()
    {
        // Act
        IReadOnlyList<JallerFolder>? folderPath = IFolderManagerExtensions.GetFolderPath(
            this.Core.Folders,
            null,
            MetadataPolicy.Public
        );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 0, folderPath.Count );
    }

    /// <summary>
    /// Ensures if the root folder ID is passed in, an empty list is returned.
    /// </summary>
    [TestMethod]
    public void ZeroRootFolderTest()
    {
        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Folders.GetFolderPath( null, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 0, folderPath.Count );
    }

    /// <summary>
    /// Ensures if a folder does not exist, we get a null back.
    /// </summary>
    [TestMethod]
    public void FolderDoesNotExistTest()
    {
        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Folders.GetFolderPath( 1, MetadataPolicy.Public );

        // Check
        Assert.IsNull( folderPath );
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

        // Act
        IReadOnlyList<JallerFolder>? folderPath = this.Core.Folders.GetFolderPath( newFolder, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath );
        Assert.AreEqual( 1, folderPath.Count );

        Assert.AreEqual( newFolder, folderPath[0] );
    }

    [TestMethod]
    public void TwoFoldersInPathTest()
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

        // Act
        IReadOnlyList<JallerFolder>? folderPath1 = this.Core.Folders.GetFolderPath( newFolder1, MetadataPolicy.Public );
        IReadOnlyList<JallerFolder>? folderPath2 = this.Core.Folders.GetFolderPath( newFolder2, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath1 );
        Assert.AreEqual( 1, folderPath1.Count );
        Assert.AreEqual( newFolder1, folderPath1[0] );

        Assert.IsNotNull( folderPath2 );
        Assert.AreEqual( 2, folderPath2.Count );
        Assert.AreEqual( newFolder1, folderPath2[0] ); // First slot should be closest to the root.
        Assert.AreEqual( newFolder2, folderPath2[1] );
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

        // Act
        IReadOnlyList<JallerFolder>? folderPath1 = this.Core.Folders.GetFolderPath( newFolder1, MetadataPolicy.Public );
        IReadOnlyList<JallerFolder>? folderPath2 = this.Core.Folders.GetFolderPath( newFolder2, MetadataPolicy.Public );
        IReadOnlyList<JallerFolder>? folderPath3 = this.Core.Folders.GetFolderPath( newFolder3, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath1 );
        Assert.AreEqual( 1, folderPath1.Count );
        Assert.AreEqual( newFolder1, folderPath1[0] );

        Assert.IsNotNull( folderPath2 );
        Assert.AreEqual( 2, folderPath2.Count );
        Assert.AreEqual( newFolder1, folderPath2[0] ); // First slot should be closest to the root.
        Assert.AreEqual( newFolder2, folderPath2[1] );

        Assert.IsNotNull( folderPath3 );
        Assert.AreEqual( 3, folderPath3.Count );
        Assert.AreEqual( newFolder1, folderPath3[0] ); // First slot should be closest to the root.
        Assert.AreEqual( newFolder2, folderPath3[1] );
        Assert.AreEqual( newFolder3, folderPath3[2] );
    }

    /// <summary>
    /// Ensures if there are multiple folders at the root,
    /// we only get the one folder we care about.
    /// </summary>
    [TestMethod]
    public void MultipleSingleFoldersAtRootTest()
    {
        // Setup
        var newFolder1 = new JallerFolder
        {
            Name = "Test Folder",
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
            ParentFolder = null,
            MetadataPrivacy = MetadataPolicy.Public
        };

        newFolder2 = newFolder2 with
        {
            Id = this.Core.Folders.ConfigureFolder( newFolder2 )
        };

        // Act
        IReadOnlyList<JallerFolder>? folderPath1 = this.Core.Folders.GetFolderPath( newFolder1, MetadataPolicy.Public );
        IReadOnlyList<JallerFolder>? folderPath2 = this.Core.Folders.GetFolderPath( newFolder2, MetadataPolicy.Public );

        // Check
        Assert.IsNotNull( folderPath1 );
        Assert.AreEqual( 1, folderPath1.Count );
        Assert.AreEqual( newFolder1, folderPath1[0] );

        Assert.IsNotNull( folderPath2 );
        Assert.AreEqual( 1, folderPath2.Count );
        Assert.AreEqual( newFolder2, folderPath2[0] );
    }
}
