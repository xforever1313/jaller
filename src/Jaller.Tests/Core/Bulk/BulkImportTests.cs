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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaller.Core.Configuration;
using Jaller.Core;
using Jaller.Tests.Mocks;
using System.Xml.Linq;
using Jaller.Standard.FileManagement;
using Jaller.Standard.Bulk;
using Jaller.Standard.FolderManagement;

namespace Jaller.Tests.Core.Bulk
{
    [TestClass]
    public sealed class BulkImportTests
    {
        // ---------------- Fields ----------------

        private JallerConfig? config;

        private JallerCore? exportCore;
        private JallerCore? importCore;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.config = new JallerConfig();
            this.config.UseInMemoryDatabase();

            this.exportCore = new JallerCore( this.config, new StubLogger() );
            this.exportCore.Init();

            this.importCore = new JallerCore( this.config, new StubLogger() );
            this.importCore.Init();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.exportCore?.Dispose();
            this.importCore?.Dispose();
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

        /// <summary>
        /// Core used to add files and folders to, and then export it to an
        /// XML file.
        /// </summary>
        public JallerCore ExportCore
        {
            get
            {
                Assert.IsNotNull( this.exportCore );
                return this.exportCore;
            }
        }

        /// <summary>
        /// Core used to import XML files into and compare with the starting
        /// <see cref="ExportCore"/>.
        /// </summary>
        public JallerCore ImportCore
        {
            get
            {
                Assert.IsNotNull( this.importCore );
                return this.importCore;
            }
        }

        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures if we import/export an empty database,
        /// nothing bad happens, and the import core is bascially a giant no-op.
        /// </summary>
        [TestMethod]
        public void EmptyDatabaseTest()
        {
            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );

            // Check
            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 0, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 0, this.ImportCore.Files.GetFileCount() );
        }

        [TestMethod]
        public void OneFileAtRootTest()
        {
            // Setup
            var file = new JallerFile
            {
                CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
                Name = "Theater Merit Badge Requirements.pdf",
                Description = "Theater Merit Badge Requirements for Scouting America",
                Details = "Requirements for the Theater Merit Badge",
                ParentFolder = null,
                Tags = new TagSet { "Scouting America", "BSA" },
                Title = "Theater Merit Badge",
            };
            this.ExportCore.Files.ConfigureFile( file );

            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );
            JallerFile? actualFile = this.ImportCore.Files.TryGetFile( file.CidV1 );

            // Check
            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 0, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 1, this.ImportCore.Files.GetFileCount() );
            Assert.AreEqual( file, actualFile );
        }

        [TestMethod]
        public void OneFolderAtRootTest()
        {
            // Setup
            var folder = new JallerFolder
            {
                Name = "Some Folder",
                ParentFolder = null
            };
            folder = folder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( folder )
            };

            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );
            JallerFolder? actualFolder = this.ImportCore.Folders.TryGetFolderByName( folder.ParentFolder, folder.Name );

            // Check
            Assert.IsNotNull( actualFolder );

            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 1, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 0, this.ImportCore.Files.GetFileCount() );
            Assert.AreEqual( folder, actualFolder );
        }

        [TestMethod]
        public void OneFolderAtRootWithPublicSettingsTest()
        {
            // Setup
            var folder = new JallerFolder
            {
                Name = "Some Folder",
                ParentFolder = null,
                DownloadablePolicy = DownloadPolicy.Public,
                MetadataPrivacy = MetadataPolicy.Public
            };
            folder = folder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( folder )
            };

            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );
            JallerFolder? actualFolder = this.ImportCore.Folders.TryGetFolderByName( folder.ParentFolder, folder.Name );

            // Check
            Assert.IsNotNull( actualFolder );

            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 1, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 0, this.ImportCore.Files.GetFileCount() );
            Assert.AreEqual( folder, actualFolder );
        }

        [TestMethod]
        public void NestedFoldersTest()
        {
            // Setup
            var parentFolder = new JallerFolder
            {
                Name = "Parent Folder",
                ParentFolder = null
            };
            parentFolder = parentFolder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( parentFolder )
            };

            var childFolder = new JallerFolder
            {
                Name = "Child Folder",
                ParentFolder = parentFolder.Id
            };
            childFolder = childFolder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( childFolder )
            };

            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );
            JallerFolder? actualParentFolder = this.ImportCore.Folders.TryGetFolderByName( parentFolder.ParentFolder, parentFolder.Name );
            JallerFolder? actualChildFolder = this.ImportCore.Folders.TryGetFolderByName( childFolder.ParentFolder, childFolder.Name );

            // Check
            Assert.IsNotNull( actualParentFolder );
            Assert.IsNotNull( actualChildFolder );

            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 2, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 0, this.ImportCore.Files.GetFileCount() );
            Assert.AreEqual( parentFolder, actualParentFolder );
            Assert.AreEqual( childFolder, actualChildFolder );
        }

        [TestMethod]
        public void NestedFilesTest()
        {
            // Setup
            var parentFolder = new JallerFolder
            {
                Name = "Parent Folder",
                ParentFolder = null
            };
            parentFolder = parentFolder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( parentFolder )
            };

            var childFolder = new JallerFolder
            {
                Name = "Child Folder",
                ParentFolder = parentFolder.Id
            };
            childFolder = childFolder with
            {
                Id = this.ExportCore.Folders.ConfigureFolder( childFolder )
            };


            // Setup
            var rootFile = new JallerFile
            {
                CidV1 = "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u",
                Name = "Theater Merit Badge Requirements.pdf",
                Description = "Theater Merit Badge Requirements for Scouting America",
                ParentFolder = null,
                Tags = new TagSet { "Scouting America", "BSA" }
            };
            this.ExportCore.Files.ConfigureFile( rootFile );

            var parentFile = new JallerFile
            {
                CidV1 = "bafybeihgrnftiwjzqbjlv7u74ocoxbev7dwtzjqqrmcylc7h3nsbclqb2y",
                Name = "Communications.pdf",
                Description = "Communications Merit Badge Requirements for Scouting America",
                ParentFolder = parentFolder.Id
            };
            this.ExportCore.Files.ConfigureFile( parentFile );

            var childFile = new JallerFile
            {
                CidV1 = "bafybeihgh5dq6xok5qrkifxh4s3bdgn3lwonny4fut77blvchhbbudjnxu",
                Name = "Energy.pdf",
                Description = "Energy Merit Badge Requirements for Scouting America",
                ParentFolder = childFolder.Id,
                Tags = new TagSet { "Communications" }
            };
            this.ExportCore.Files.ConfigureFile( childFile );

            // Act
            XDocument doc = this.ExportCore.BulkOperations.BulkGetAllMetaData( MetadataPolicy.Private );
            Console.WriteLine( doc );

            BulkAddResult result = this.ImportCore.BulkOperations.BulkAddMetaData( doc, false );
            JallerFolder? actualParentFolder = this.ImportCore.Folders.TryGetFolderByName( parentFolder.ParentFolder, parentFolder.Name );
            JallerFolder? actualChildFolder = this.ImportCore.Folders.TryGetFolderByName( childFolder.ParentFolder, childFolder.Name );
            JallerFile? actualRootFile = this.ImportCore.Files.TryGetFile( rootFile.CidV1 );
            JallerFile? actualParentFile = this.ImportCore.Files.TryGetFile( parentFile.CidV1 );
            JallerFile? actualChildFile = this.ImportCore.Files.TryGetFile( childFile.CidV1 );

            // Check
            Assert.IsNotNull( actualParentFolder );
            Assert.IsNotNull( actualChildFolder );
            Assert.IsNotNull( actualRootFile );
            Assert.IsNotNull( actualParentFile );
            Assert.IsNotNull( actualChildFile );

            Assert.AreEqual( 0, result.Warnings.Count );
            Assert.AreEqual( 0, result.Errors.Count );
            Assert.AreEqual( 2, this.ImportCore.Folders.GetFolderCount() );
            Assert.AreEqual( 3, this.ImportCore.Files.GetFileCount() );
            Assert.AreEqual( parentFolder, actualParentFolder );
            Assert.AreEqual( childFolder, actualChildFolder );
            Assert.AreEqual( rootFile, actualRootFile );
            Assert.AreEqual( parentFile, actualParentFile );
            Assert.AreEqual( childFile, actualChildFile );
        }
    }
}
