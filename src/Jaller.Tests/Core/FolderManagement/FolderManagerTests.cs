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
using Jaller.Core.Exceptions;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;
using Jaller.Tests.Mocks;

namespace Jaller.Tests.Core.FolderManagement
{
    [TestClass]
    public sealed class FolderManagerTests
    {
        // ---------------- Fields ----------------

        private JallerConfig? config;

        private JallerCore? core;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.config = new JallerConfig();
            this.config.Database.DatabaseLocation = null;

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
        public void DeleteFolderThatDoesntExistTest()
        {
            // Setup
            var folder = new JallerFolder
            {
                Id = 1,
                Name = "test",
                ParentFolder = null
            };

            // Act
            JallerFolder? foundFolder = this.Core.Folders.TryGetFolder( folder.Id );
            this.Core.Folders.DeleteFolder( folder );

            // Check
            Assert.IsNull( foundFolder );
            Assert.AreEqual( 0, this.Core.Folders.GetFolderCount() );

        }

        [TestMethod]
        public void GetRootFolderWithNoDirectoriesOrFilesTest()
        {
            // Act
            FolderContents rootContents = this.Core.Folders.GetRootFolder( FileMetadataPolicy.Private );

            // Check
            Assert.IsNull( rootContents.ChildFolders );
            Assert.IsNull( rootContents.Files );
        }

        [TestMethod]
        public void SingleFolderCreationTest()
        {
            // Setup
            var newFolder = new JallerFolder
            {
                Name = "Test Folder",
                ParentFolder = null
            };

            // Act
            int newFolderId = this.Core.Folders.ConfigureFolder( newFolder );
            Assert.AreNotEqual( 0, newFolderId );
            newFolder = newFolder with
            {
                // Need to set the ID since it got updated, and
                // equals below will fail otherwise.
                Id = newFolderId
            };

            JallerFolder? createdFolder = this.Core.Folders.TryGetFolder( newFolderId );

            // Check
            Assert.AreEqual( 1, this.Core.Folders.GetFolderCount() );
            Assert.IsNotNull( createdFolder );
            Assert.AreEqual( newFolder, createdFolder );
        }

        [TestMethod]
        public void ModifyExistingFolderTest()
        {
            // Setup
            var newFolder = new JallerFolder
            {
                Name = "Test Folder",
                ParentFolder = null
            };

            // Act
            int newFolderId = this.Core.Folders.ConfigureFolder( newFolder );
            Assert.AreNotEqual( 0, newFolderId );
            newFolder = newFolder with
            {
                // Need to set the ID since it got updated, and
                // equals below will fail otherwise.
                Id = newFolderId,
                Name = "Changed folder"
            };

            int newFolderId2 = this.Core.Folders.ConfigureFolder( newFolder );
            JallerFolder? createdFolder = this.Core.Folders.TryGetFolder( newFolderId2 );

            // Check
            Assert.AreEqual( 1, this.Core.Folders.GetFolderCount() );
            Assert.AreEqual( newFolderId, newFolderId2 );
            Assert.IsNotNull( createdFolder );
            Assert.AreEqual( newFolder, createdFolder );
        }

        [TestMethod]
        public void NoChangesTest()
        {
            // Setup
            var newFolder = new JallerFolder
            {
                Name = "Test Folder",
                ParentFolder = null
            };

            // Act
            int newFolderId = this.Core.Folders.ConfigureFolder( newFolder );
            Assert.AreNotEqual( 0, newFolderId );
            newFolder = newFolder with
            {
                // Need to set the ID since it got updated, and
                // equals below will fail otherwise.
                Id = newFolderId,
            };

            int newFolderId2 = this.Core.Folders.ConfigureFolder( newFolder );
            JallerFolder? createdFolder = this.Core.Folders.TryGetFolder( newFolderId2 );

            // Check
            Assert.AreEqual( 1, this.Core.Folders.GetFolderCount() );
            Assert.AreEqual( newFolderId, newFolderId2 );
            Assert.IsNotNull( createdFolder );
            Assert.AreEqual( newFolder, createdFolder );
        }

        [TestMethod]
        public void CreateThenDeleteSingleFolderTest()
        {
            // Setup
            var newFolder = new JallerFolder
            {
                Name = "Test Folder",
                ParentFolder = null
            };

            // Act
            int newFolderId = this.Core.Folders.ConfigureFolder( newFolder );
            Assert.AreNotEqual( 0, newFolderId );
            Assert.AreEqual( 1, this.Core.Folders.GetFolderCount() ); // <- Sanity check

            this.Core.Folders.DeleteFolder( newFolderId );
            JallerFolder? createdFolder = this.Core.Folders.TryGetFolder( newFolderId );

            // Check
            Assert.AreEqual( 0, this.Core.Folders.GetFolderCount() );
            Assert.IsNull( createdFolder );
        }

        [TestMethod]
        public void CreateFolderTreeTest()
        {
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

            var child2 = new JallerFolder
            {
                Name = "Child 2",
                ParentFolder = rootFolder.Id
            };

            child2 = child2 with
            {
                Id = this.Core.Folders.ConfigureFolder( child2 )
            };
            Assert.AreNotEqual( 0, child2.Id );

            // Setup grandchildren
            var grandchild1 = new JallerFolder
            {
                Name = "Grandchild 1",
                ParentFolder = child1.Id
            };

            grandchild1 = grandchild1 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild1 )
            };
            Assert.AreNotEqual( 0, grandchild1.Id );

            var grandchild2 = new JallerFolder
            {
                Name = "Grandchild 2",
                ParentFolder = child1.Id
            };

            grandchild2 = grandchild2 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild2 )
            };
            Assert.AreNotEqual( 0, grandchild2.Id );

            // Act
            int totalFolders = this.Core.Folders.GetFolderCount();
            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );
            FolderContents? child2Contents = this.Core.Folders.TryGetFolderContents( child2.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild1Contents = this.Core.Folders.TryGetFolderContents( grandchild1.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild2Contents = this.Core.Folders.TryGetFolderContents( grandchild2.Id, FileMetadataPolicy.Private );

            // Check
            Assert.AreEqual( 5, totalFolders );
            Assert.AreEqual( rootFolder, this.Core.Folders.TryGetFolder( rootFolder.Id ) );
            Assert.AreEqual( child1, this.Core.Folders.TryGetFolder( child1.Id ) );
            Assert.AreEqual( child2, this.Core.Folders.TryGetFolder( child2.Id ) );
            Assert.AreEqual( grandchild1, this.Core.Folders.TryGetFolder( grandchild1.Id ) );
            Assert.AreEqual( grandchild2, this.Core.Folders.TryGetFolder( grandchild2.Id ) );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.Files );
            Assert.IsNotNull( rootFolderContents.ChildFolders );
            Assert.AreEqual( 2, rootFolderContents.ChildFolders.Count );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child1 ) );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child2 ) );

            Assert.IsNotNull( child1Contents );
            Assert.IsNull( child1Contents.Files );
            Assert.IsNotNull( child1Contents.ChildFolders );
            Assert.AreEqual( 2, child1Contents.ChildFolders.Count );
            Assert.IsTrue( child1Contents.ChildFolders.Contains( grandchild1 ) );
            Assert.IsTrue( child1Contents.ChildFolders.Contains( grandchild2 ) );

            Assert.IsNotNull( child2Contents );
            Assert.IsNull( child2Contents.Files );
            Assert.IsNull( child2Contents.ChildFolders );

            Assert.IsNotNull( grandchild1Contents );
            Assert.IsNull( grandchild1Contents.Files );
            Assert.IsNull( grandchild1Contents.ChildFolders );

            Assert.IsNotNull( grandchild2Contents );
            Assert.IsNull( grandchild2Contents.Files );
            Assert.IsNull( grandchild2Contents.ChildFolders );
        }

        [TestMethod]
        public void DeleteRootFolderTest()
        {
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

            var child2 = new JallerFolder
            {
                Name = "Child 2",
                ParentFolder = rootFolder.Id
            };

            child2 = child2 with
            {
                Id = this.Core.Folders.ConfigureFolder( child2 )
            };
            Assert.AreNotEqual( 0, child2.Id );

            // Setup grandchildren
            var grandchild1 = new JallerFolder
            {
                Name = "Grandchild 1",
                ParentFolder = child1.Id
            };

            grandchild1 = grandchild1 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild1 )
            };
            Assert.AreNotEqual( 0, grandchild1.Id );

            var grandchild2 = new JallerFolder
            {
                Name = "Grandchild 2",
                ParentFolder = child1.Id
            };

            grandchild2 = grandchild2 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild2 )
            };
            Assert.AreNotEqual( 0, grandchild2.Id );

            // Act
            int beforeFolderCount = this.Core.Folders.GetFolderCount();
            this.Core.Folders.DeleteFolder( rootFolder );
            int afterFolderCount = this.Core.Folders.GetFolderCount();

            // Check
            Assert.AreEqual( 5, beforeFolderCount );
            Assert.AreEqual( 0, afterFolderCount );
        }

        [TestMethod]
        public void DeleteChildFolderTest()
        {
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

            var child2 = new JallerFolder
            {
                Name = "Child 2",
                ParentFolder = rootFolder.Id
            };

            child2 = child2 with
            {
                Id = this.Core.Folders.ConfigureFolder( child2 )
            };
            Assert.AreNotEqual( 0, child2.Id );

            // Setup grandchildren
            var grandchild1 = new JallerFolder
            {
                Name = "Grandchild 1",
                ParentFolder = child1.Id
            };

            grandchild1 = grandchild1 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild1 )
            };
            Assert.AreNotEqual( 0, grandchild1.Id );

            var grandchild2 = new JallerFolder
            {
                Name = "Grandchild 2",
                ParentFolder = child1.Id
            };

            grandchild2 = grandchild2 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild2 )
            };
            Assert.AreNotEqual( 0, grandchild2.Id );

            // Act
            int beforeFolderCount = this.Core.Folders.GetFolderCount();
            this.Core.Folders.DeleteFolder( child1 );
            int afterFolderCount = this.Core.Folders.GetFolderCount();

            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );
            FolderContents? child2Contents = this.Core.Folders.TryGetFolderContents( child2.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild1Contents = this.Core.Folders.TryGetFolderContents( grandchild1.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild2Contents = this.Core.Folders.TryGetFolderContents( grandchild2.Id, FileMetadataPolicy.Private );

            // Check
            Assert.AreEqual( 5, beforeFolderCount );
            // Should be 2: The root and the second child.
            Assert.AreEqual( 2, afterFolderCount );

            // Check
            Assert.AreEqual( rootFolder, this.Core.Folders.TryGetFolder( rootFolder.Id ) );
            Assert.IsNull( this.Core.Folders.TryGetFolder( child1.Id ) );
            Assert.AreEqual( child2, this.Core.Folders.TryGetFolder( child2.Id ) );
            Assert.IsNull( this.Core.Folders.TryGetFolder( grandchild1.Id ) );
            Assert.IsNull( this.Core.Folders.TryGetFolder( grandchild2.Id ) );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.Files );
            Assert.IsNotNull( rootFolderContents.ChildFolders );
            Assert.AreEqual( 1, rootFolderContents.ChildFolders.Count );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child2 ) );

            Assert.IsNull( child1Contents );

            Assert.IsNotNull( child2Contents );
            Assert.IsNull( child2Contents.Files );
            Assert.IsNull( child2Contents.ChildFolders );

            Assert.IsNull( grandchild1Contents );
            Assert.IsNull( grandchild2Contents );
        }

        [TestMethod]
        public void DeleteGrandChildTest()
        {
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

            var child2 = new JallerFolder
            {
                Name = "Child 2",
                ParentFolder = rootFolder.Id
            };

            child2 = child2 with
            {
                Id = this.Core.Folders.ConfigureFolder( child2 )
            };
            Assert.AreNotEqual( 0, child2.Id );

            // Setup grandchildren
            var grandchild1 = new JallerFolder
            {
                Name = "Grandchild 1",
                ParentFolder = child1.Id
            };

            grandchild1 = grandchild1 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild1 )
            };
            Assert.AreNotEqual( 0, grandchild1.Id );

            var grandchild2 = new JallerFolder
            {
                Name = "Grandchild 2",
                ParentFolder = child1.Id
            };

            grandchild2 = grandchild2 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild2 )
            };
            Assert.AreNotEqual( 0, grandchild2.Id );

            // Act
            int beforeFolderCount = this.Core.Folders.GetFolderCount();
            this.Core.Folders.DeleteFolder( grandchild2 );
            int afterFolderCount = this.Core.Folders.GetFolderCount();

            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );
            FolderContents? child2Contents = this.Core.Folders.TryGetFolderContents( child2.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild1Contents = this.Core.Folders.TryGetFolderContents( grandchild1.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild2Contents = this.Core.Folders.TryGetFolderContents( grandchild2.Id, FileMetadataPolicy.Private );

            // Check
            Assert.AreEqual( 5, beforeFolderCount );
            Assert.AreEqual( 4, afterFolderCount );

            Assert.AreEqual( rootFolder, this.Core.Folders.TryGetFolder( rootFolder.Id ) );
            Assert.AreEqual( child1, this.Core.Folders.TryGetFolder( child1.Id ) );
            Assert.AreEqual( child2, this.Core.Folders.TryGetFolder( child2.Id ) );
            Assert.AreEqual( grandchild1, this.Core.Folders.TryGetFolder( grandchild1.Id ) );
            Assert.IsNull( this.Core.Folders.TryGetFolder( grandchild2.Id ) );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.Files );
            Assert.IsNotNull( rootFolderContents.ChildFolders );
            Assert.AreEqual( 2, rootFolderContents.ChildFolders.Count );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child1 ) );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child2 ) );

            Assert.IsNotNull( child1Contents );
            Assert.IsNull( child1Contents.Files );
            Assert.IsNotNull( child1Contents.ChildFolders );
            Assert.AreEqual( 1, child1Contents.ChildFolders.Count );
            Assert.IsTrue( child1Contents.ChildFolders.Contains( grandchild1 ) );

            Assert.IsNotNull( child2Contents );
            Assert.IsNull( child2Contents.Files );
            Assert.IsNull( child2Contents.ChildFolders );

            Assert.IsNotNull( grandchild1Contents );
            Assert.IsNull( grandchild1Contents.Files );
            Assert.IsNull( grandchild1Contents.ChildFolders );

            Assert.IsNull( grandchild2Contents );
        }

        /// <summary>
        /// If a folder had a child folder, but its missing for some reason
        /// from the database, it will garbage collect it.
        /// </summary>
        [TestMethod]
        public void OrphanedChildTest()
        {
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

            this.Core.Database.Directories.Delete( child1.Id );

            // Act
            int totalFolders = this.Core.Folders.GetFolderCount();
            Assert.AreEqual( 1, totalFolders );
            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );

            // Check
            totalFolders = this.Core.Folders.GetFolderCount();
            Assert.AreEqual( 1, totalFolders );
            Assert.AreEqual( rootFolder, this.Core.Folders.TryGetFolder( rootFolder.Id ) );
            Assert.IsNull( this.Core.Folders.TryGetFolder( child1.Id ) );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.Files );
            Assert.IsNull( rootFolderContents.ChildFolders );

            Assert.IsNull( child1Contents );
        }

        [TestMethod]
        public void CreateFolderWithNonExistentParentFolderTest()
        {
            // Setup
            var folder = new JallerFolder
            {
                Name = "New Folder",
                ParentFolder = 100
            };

            // Act
            Assert.ThrowsException<FolderNotFoundException>( () => this.Core.Folders.ConfigureFolder( folder ) );

            // Check
            Assert.AreEqual( 0, this.Core.Folders.GetFolderCount() );
        }

        /// <summary>
        /// If updating a folder results in a parent folder that does not exist,
        /// </summary>
        [TestMethod]
        public void OrphanedParentTest()
        {
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

            // Delete root folder.
            this.Core.Database.Directories.Delete( rootFolder.Id );

            child1 = child1 with
            {
                Name = "New Name"
            };

            JallerFolder expectedChild1 = child1 with
            {
                ParentFolder = null
            };

            // Act
            this.Core.Folders.ConfigureFolder( child1 );
            int folderCount = this.Core.Folders.GetFolderCount();
            JallerFolder? actualChild1 = this.Core.Folders.TryGetFolder( child1.Id );

            // Check
            Assert.AreEqual( folderCount, this.Core.Folders.GetFolderCount() );
            Assert.IsNotNull( actualChild1 );
            Assert.AreEqual( expectedChild1, actualChild1 );
        }

        [TestMethod]
        public void MoveFolderTreeTest()
        {
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

            var child2 = new JallerFolder
            {
                Name = "Child 2",
                ParentFolder = rootFolder.Id
            };

            child2 = child2 with
            {
                Id = this.Core.Folders.ConfigureFolder( child2 )
            };
            Assert.AreNotEqual( 0, child2.Id );

            // Setup grandchildren
            var grandchild1 = new JallerFolder
            {
                Name = "Grandchild 1",
                ParentFolder = child1.Id
            };

            grandchild1 = grandchild1 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild1 )
            };
            Assert.AreNotEqual( 0, grandchild1.Id );

            var grandchild2 = new JallerFolder
            {
                Name = "Grandchild 2",
                ParentFolder = child1.Id
            };

            grandchild2 = grandchild2 with
            {
                Id = this.Core.Folders.ConfigureFolder( grandchild2 )
            };
            Assert.AreNotEqual( 0, grandchild2.Id );

            // Act - Move grandchild 2 to child 2.
            grandchild2 = grandchild2 with
            {
                ParentFolder = child2.Id
            };
            this.Core.Folders.ConfigureFolder( grandchild2 );

            int totalFolders = this.Core.Folders.GetFolderCount();
            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );
            FolderContents? child2Contents = this.Core.Folders.TryGetFolderContents( child2.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild1Contents = this.Core.Folders.TryGetFolderContents( grandchild1.Id, FileMetadataPolicy.Private );
            FolderContents? grandchild2Contents = this.Core.Folders.TryGetFolderContents( grandchild2.Id, FileMetadataPolicy.Private );

            // Check
            Assert.AreEqual( 5, totalFolders );
            Assert.AreEqual( rootFolder, this.Core.Folders.TryGetFolder( rootFolder.Id ) );
            Assert.AreEqual( child1, this.Core.Folders.TryGetFolder( child1.Id ) );
            Assert.AreEqual( child2, this.Core.Folders.TryGetFolder( child2.Id ) );
            Assert.AreEqual( grandchild1, this.Core.Folders.TryGetFolder( grandchild1.Id ) );
            Assert.AreEqual( grandchild2, this.Core.Folders.TryGetFolder( grandchild2.Id ) );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.Files );
            Assert.IsNotNull( rootFolderContents.ChildFolders );
            Assert.AreEqual( 2, rootFolderContents.ChildFolders.Count );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child1 ) );
            Assert.IsTrue( rootFolderContents.ChildFolders.Contains( child2 ) );

            Assert.IsNotNull( child1Contents );
            Assert.IsNull( child1Contents.Files );
            Assert.IsNotNull( child1Contents.ChildFolders );
            Assert.AreEqual( 1, child1Contents.ChildFolders.Count );
            Assert.IsTrue( child1Contents.ChildFolders.Contains( grandchild1 ) );
            Assert.IsFalse( child1Contents.ChildFolders.Contains( grandchild2 ) );

            Assert.IsNotNull( child2Contents );
            Assert.IsNull( child2Contents.Files );
            Assert.IsNotNull( child2Contents.ChildFolders );
            Assert.AreEqual( 1, child2Contents.ChildFolders.Count );
            Assert.IsFalse( child2Contents.ChildFolders.Contains( grandchild1 ) );
            Assert.IsTrue( child2Contents.ChildFolders.Contains( grandchild2 ) );

            Assert.IsNotNull( grandchild1Contents );
            Assert.IsNull( grandchild1Contents.Files );
            Assert.IsNull( grandchild1Contents.ChildFolders );

            Assert.IsNotNull( grandchild2Contents );
            Assert.IsNull( grandchild2Contents.Files );
            Assert.IsNull( grandchild2Contents.ChildFolders );
        }

        [TestMethod]
        public void MoveFolderToRootTest()
        {
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

            // Act
            child1 = child1 with
            {
                ParentFolder = null
            };
            this.Core.Folders.ConfigureFolder( child1 );

            // Check
            int totalFolders = this.Core.Folders.GetFolderCount();
            FolderContents? rootFolderContents = this.Core.Folders.TryGetFolderContents( rootFolder.Id, FileMetadataPolicy.Private );
            FolderContents? child1Contents = this.Core.Folders.TryGetFolderContents( child1.Id, FileMetadataPolicy.Private );

            // Check
            Assert.AreEqual( 2, totalFolders );

            Assert.IsNotNull( rootFolderContents );
            Assert.IsNull( rootFolderContents.ChildFolders );
            Assert.IsNull( rootFolderContents.Files );

            Assert.IsNotNull( child1Contents );
            Assert.IsNull( child1Contents.ChildFolders );
            Assert.IsNull( child1Contents.Files );
        }
    }
}
