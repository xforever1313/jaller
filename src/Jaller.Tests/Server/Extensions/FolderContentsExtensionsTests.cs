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

using Jaller.Contracts.FileManagement;
using Jaller.Contracts.FolderManagement;
using Jaller.Server.Extensions;
using Jaller.Standard.FileManagement;
using Jaller.Standard.FolderManagement;

namespace Jaller.Tests.Server.Extensions
{
    [TestClass]
    public sealed class FolderContentsExtensionsTests
    {
        // ---------------- Tests ----------------

        [TestMethod]
        public void ToFolderContentsInfoTests()
        {
            // Setup
            var originalFolder = new JallerFolder
            {
                Id = 1,
                Name = "Some Folder",
                ParentFolder = null
            };

            var originalFile = new JallerFile
            {
                CidV1 = "QmeF4NuE7sQLdDiVznp8fvkkRtkbL6MvsrmRohReSKLrf5",
                Description = "Scounting America Programming Merit Badge",
                Name = "Programming Merit Badge Requirements",
                ParentFolder = originalFolder.Id,
                MimeType = "application/pdf"
            };

            var originalContents = new FolderContents
            {
                ChildFolders = new JallerFolder[] { originalFolder },
                Files = new JallerFile[] { originalFile }
            };

            var expectedFolder = new JallerFolderInfo
            {
                Id = originalFolder.Id,
                Name = originalFolder.Name,
                ParentFolderId = originalFolder.ParentFolder
            };

            var expectedFile = new JallerFileTreeLeafInfo
            {
                CidV1 = originalFile.CidV1,
                Name = originalFile.Name,
                ParentFolderId = originalFile.ParentFolder,
            };

            var expectedContents = new JallerFolderTreeContentsInfo
            {
                FolderId = originalFolder.Id,
                Files = new JallerFileTreeLeafInfo[] { expectedFile },
                Folders = new JallerFolderInfo[] { expectedFolder }
            };

            // Act
            JallerFolderTreeContentsInfo actualInfo = originalContents.ToFolderContentsInfo( originalFolder.Id );

            // Check
            Assert.AreEqual( originalFolder.Id, actualInfo.FolderId );

            Assert.IsNotNull( actualInfo.Files );
            Assert.AreEqual( 1, actualInfo.Files.Length );
            Assert.AreEqual( expectedFile, actualInfo.Files.First() );

            Assert.IsNotNull( actualInfo.Folders );
            Assert.AreEqual( 1, actualInfo.Folders.Length );
            Assert.AreEqual( expectedFolder, actualInfo.Folders.First() );
        }

        [TestMethod]
        public void EmptyRootFolderContentsTest()
        {
            // Setup
            var expectedContents = new JallerFolderTreeContentsInfo
            {
                Files = null,
                FolderId = null,
                Folders = null
            };

            var originalContents = new FolderContents
            {
                ChildFolders = null,
                Files = null
            };

            // Act
            JallerFolderTreeContentsInfo actualInfo = originalContents.ToFolderContentsInfo( null );

            // Check
            Assert.IsNull( actualInfo.FolderId );
            Assert.IsNull( actualInfo.Files );
            Assert.IsNull( actualInfo.Folders );
        }
    }
}
