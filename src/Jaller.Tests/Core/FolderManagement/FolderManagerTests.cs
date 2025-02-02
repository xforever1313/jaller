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
using Jaller.Core;
using Jaller.Core.Configuration;
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
    }
}
