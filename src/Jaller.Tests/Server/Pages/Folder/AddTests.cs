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

using System.Net;
using Jaller.Core;
using Jaller.Core.Configuration;
using Jaller.Server.Pages.Folder;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jaller.Tests.Server.Pages.Folder;

[TestClass]
public sealed class AddTests
{
    // ---------------- Fields ----------------

    private JallerConfig? config;

    private JallerCore? core;

    private AddModel? uut;

    // ---------------- Setup / Teardown ----------------

    [TestInitialize]
    public void TestSetup()
    {
        this.config = new JallerConfig();
        this.config.UseInMemoryDatabase();

        this.core = new JallerCore( this.config, new StubLogger() );
        this.core.Init();

        this.uut = new AddModel( this.core );

        var httpContext = new DefaultHttpContext();
        this.uut.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    [TestCleanup]
    public void TestTeardown()
    {
        this.core?.Dispose();
    }

    // ---------------- Properties ----------------

    private JallerCore Core
    {
        get
        {
            ArgumentNullException.ThrowIfNull( this.core );
            return this.core;
        }
    }

    private AddModel Uut
    {
        get
        {
            ArgumentNullException.ThrowIfNull( this.uut );
            return this.uut;
        }
    }

    // ---------------- Tests ----------------

    /// <summary>
    /// Ensures the model is correct if the parent ID is null (representing root folder).
    /// </summary>
    [TestMethod]
    public void GetWhenParentFolderIsNullTest()
    {
        // Setup
        int? parentFolderId = null;

        // Act
        IActionResult result = this.Uut.OnGetAsync( parentFolderId ).Result;

        // Check
        PageResult? pageResult = result as PageResult;
        Assert.IsNotNull( pageResult );
        Assert.AreEqual( (int)HttpStatusCode.OK, this.Uut.Response.StatusCode );

        // Parent folder should be null, since its the root directory.  There should not
        // be an error message.
        Assert.IsNull( this.Uut.ParentFolder );
        Assert.IsNull( this.Uut.GetRequestErrorMessage );
    }

    /// <summary>
    /// Ensures the model is correct if the parent ID is zero (representing root folder).
    /// </summary>
    [TestMethod]
    public void GetWhenParentFolderIsZeroTest()
    {
        // Setup
        int? parentFolderId = 0;

        // Act
        IActionResult result = this.Uut.OnGetAsync( parentFolderId ).Result;

        // Check
        PageResult? pageResult = result as PageResult;
        Assert.IsNotNull( pageResult );
        Assert.AreEqual( (int)HttpStatusCode.OK, this.Uut.Response.StatusCode );

        // Parent folder should be null, since its the root directory.
        Assert.IsNull( this.Uut.ParentFolder );
        Assert.IsNull( this.Uut.GetRequestErrorMessage );
    }

    /// <summary>
    /// Ensures the model is correct if the parent ID does not exist.
    /// </summary>
    [TestMethod]
    public void GetWhenParentFolderDoesNotExistTest()
    {
        // Setup
        int? parentFolderId = 1;

        // Act
        IActionResult result = this.Uut.OnGetAsync( parentFolderId ).Result;

        // Check
        PageResult? pageResult = result as PageResult;
        Assert.IsNotNull( pageResult );
        Assert.AreEqual( (int)HttpStatusCode.NotFound, this.Uut.Response.StatusCode );

        // Parent folder should be null, we didn't find it.
        Assert.IsNull( this.Uut.ParentFolder );

        // There should be some kind of error message.
        Assert.IsFalse( string.IsNullOrWhiteSpace( this.Uut.GetRequestErrorMessage ) );
    }

    /// <summary>
    /// If a user does a POST request with no values specified,
    /// it does the default behavior of adding a locked-down untitled folder to the root directory.
    /// </summary>
    [TestMethod]
    public void PostWithDefaultValuesTest()
    {
        // Setup
        var expectedFolder = new JallerFolder
        {
            Id = 1,
            Name = "Untitled Folder",
            ParentFolder = null,
            DownloadablePolicy = JallerFolder.DefaultDownloadablePolicy,
            MetadataPrivacy = JallerFolder.DefaultMetadataPrivacy
        };

        // Act
        IActionResult result = this.Uut.OnPostAsync().Result;
        JallerFolder? folder = this.Core.Folders.TryGetFolder( 1 );

        // Check
        Assert.IsFalse( string.IsNullOrWhiteSpace( this.Uut.InfoMessage ) );
        Assert.IsNull( this.Uut.WarningMessage );
        Assert.IsNull( this.Uut.ErrorMessage );

        Assert.IsNotNull( folder );
        Assert.AreEqual( expectedFolder, folder );
    }

    /// <summary>
    /// If a user does a POST request with no values specified,
    /// it does the default behavior of adding a locked-down untitled folder to the root directory.
    /// </summary>
    [TestMethod]
    public void AddToRootFolderTest()
    {
        // Setup
        var expectedFolder = new JallerFolder
        {
            Id = 1,
            Name = "My Folder",
            ParentFolder = null,
            DownloadablePolicy = DownloadPolicy.Public,
            MetadataPrivacy = MetadataPolicy.Public
        };

        SetModelToFolder( expectedFolder );

        // Act
        IActionResult result = this.Uut.OnPostAsync().Result;
        JallerFolder? folder = this.Core.Folders.TryGetFolder( expectedFolder.Id );

        // Check
        Assert.IsFalse( string.IsNullOrWhiteSpace( this.Uut.InfoMessage ) );
        Assert.IsNull( this.Uut.WarningMessage );
        Assert.IsNull( this.Uut.ErrorMessage );

        Assert.IsNotNull( folder );
        Assert.AreEqual( expectedFolder, folder );
    }

    /// <summary>
    /// If a user does a POST request with no values specified,
    /// it does the default behavior of adding a locked-down untitled folder to the root directory.
    /// </summary>
    [TestMethod]
    public void AddToSubfolderFolderTest()
    {
        // Setup
        var rootFolder = new JallerFolder
        {
            Name = "Root",
            ParentFolder = null
        };
        rootFolder = rootFolder with
        {
            Id = this.Core.Folders.ConfigureFolder( rootFolder )
        };

        var expectedFolder = new JallerFolder
        {
            Id = rootFolder.Id + 1,
            Name = "My Sub-Folder",
            ParentFolder = rootFolder.Id,
            DownloadablePolicy = DownloadPolicy.Public,
            MetadataPrivacy = MetadataPolicy.Public
        };

        SetModelToFolder( expectedFolder );

        // Act
        IActionResult result = this.Uut.OnPostAsync().Result;
        JallerFolder? folder = this.Core.Folders.TryGetFolder( expectedFolder.Id );

        // Check
        Assert.IsFalse( string.IsNullOrWhiteSpace( this.Uut.InfoMessage ) );
        Assert.IsNull( this.Uut.WarningMessage );
        Assert.IsNull( this.Uut.ErrorMessage );

        Assert.IsNotNull( folder );
        Assert.AreEqual( expectedFolder, folder );
    }

    /// <summary>
    /// Ensures if we try to add to a sub-folder that doesn't exist,
    /// we get an error message.
    /// </summary>
    [TestMethod]
    public void AddToSubFolderThatDoesntExistTest()
    {
        // Setup
        this.Uut.ParentFolderId = 1;

        // Act
        IActionResult result = this.Uut.OnPostAsync().Result;
        int folderCount = this.Core.Folders.GetFolderCount();

        // Check
        Assert.IsNull( this.Uut.InfoMessage );
        Assert.IsNull( this.Uut.WarningMessage );
        Assert.IsFalse( string.IsNullOrWhiteSpace( this.Uut.ErrorMessage ) );

        Assert.AreEqual( 0, folderCount );
    }

    // ---------------- Test Helpers ----------------

    private void SetModelToFolder( JallerFolder folder )
    {
        this.Uut.NewFolderName = folder.Name;
        this.Uut.ParentFolderId = folder.ParentFolder;
        this.Uut.MetadataPrivacy = folder.MetadataPrivacy;
        this.Uut.DownloadablePolicy = folder.DownloadablePolicy;
    }
}
