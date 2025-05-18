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

using Jaller.Server;
using Jaller.Server.Extensions;
using Jaller.Standard.FileManagement;

namespace Jaller.Tests.Server.Extensions
{
    public sealed class JallerFileExtensionsTests
    {
        // ---------------- Tests ----------------

        // -------- IsRenderable Tests --------

        /// <summary>
        /// Ensures if the mime type is derived from the file name,
        /// it returns the correct render type.
        /// </summary>
        [TestMethod]
        public void MimeTypeFromFileNameTest()
        {
            void DoTest( string fileName, RenderableMimeType expectedMimeType )
            {
                // Setup
                var uut = new JallerFile
                {
                    CidV1 = "bafybeiexvil4yq6jqcf53r5gylu5pgj34kul4cld2zc5nmth735k5pqigq",
                    Name = fileName,
                    MimeType = null,
                    ParentFolder = null
                };

                // Act
                RenderableMimeType actualMimeType = uut.IsRenderable();

                // Check
                Assert.AreEqual( expectedMimeType, actualMimeType );
            }

            DoTest( "knickerbocker_press_1927_mar_27_scout_news.pdf", RenderableMimeType.Pdf );

            DoTest( "somefile.acc", RenderableMimeType.Audio );
            DoTest( "somefile.mid", RenderableMimeType.Audio );
            DoTest( "somefile.midi", RenderableMimeType.Audio );
            DoTest( "somefile.mp3", RenderableMimeType.Audio );
            DoTest( "somefile.oga", RenderableMimeType.Audio );
            DoTest( "somefile.opus", RenderableMimeType.Audio );
            DoTest( "somefile.wav", RenderableMimeType.Audio );
            DoTest( "somefile.webm", RenderableMimeType.Audio );

            DoTest( "somefile.apng", RenderableMimeType.Image );
            DoTest( "somefile.avif", RenderableMimeType.Image );
            DoTest( "somefile.bmp", RenderableMimeType.Image );
            DoTest( "somefile.gif", RenderableMimeType.Image );
            DoTest( "somefile.ico", RenderableMimeType.Image );
            DoTest( "somefile.jpg", RenderableMimeType.Image );
            DoTest( "somefile.jpeg", RenderableMimeType.Image );
            DoTest( "somefile.png", RenderableMimeType.Image );
            DoTest( "somefile.svg", RenderableMimeType.Image );
            DoTest( "somefile.tif", RenderableMimeType.Image );
            DoTest( "somefile.tiff", RenderableMimeType.Image );
            DoTest( "somefile.webp", RenderableMimeType.Image );

            DoTest( "somefile.avi", RenderableMimeType.Video );
            DoTest( "somefile.mp4", RenderableMimeType.Video );
            DoTest( "somefile.mpeg", RenderableMimeType.Video );
            DoTest( "somefile.ogv", RenderableMimeType.Video );
            DoTest( "somefile.ts", RenderableMimeType.Video );
            DoTest( "somefile.webm", RenderableMimeType.Video );
            DoTest( "somefile.3gp", RenderableMimeType.Video );
            DoTest( "somefile.3gp2", RenderableMimeType.Video );

            DoTest( "someFile", RenderableMimeType.NotRenderable );
            DoTest( "someFile.zip", RenderableMimeType.NotRenderable );
        }

        /// <summary>
        /// Ensures if the mime type is derived from the file name,
        /// it returns the correct render type.
        /// </summary>
        [TestMethod]
        public void MimeTypeFromOverrideTest()
        {
            void DoTest( string mimeType, RenderableMimeType expectedMimeType )
            {
                // Setup
                var uut = new JallerFile
                {
                    CidV1 = "bafybeiexvil4yq6jqcf53r5gylu5pgj34kul4cld2zc5nmth735k5pqigq",
                    Name = "somefile.something",
                    MimeType = mimeType,
                    ParentFolder = null
                };

                // Act
                RenderableMimeType actualMimeType = uut.IsRenderable();

                // Check
                Assert.AreEqual( expectedMimeType, actualMimeType );
            }

            DoTest( "knickerbocker_press_1927_mar_27_scout_news.pdf", RenderableMimeType.Pdf );

            DoTest( "audio/acc", RenderableMimeType.Audio );
            DoTest( "audio/midi", RenderableMimeType.Audio );
            DoTest( "audio/mpeg", RenderableMimeType.Audio );
            DoTest( "audio/ogg", RenderableMimeType.Audio );
            DoTest( "audio/wav", RenderableMimeType.Audio );
            DoTest( "audio/webm", RenderableMimeType.Audio );

            DoTest( "image/apng", RenderableMimeType.Image );
            DoTest( "image/avif", RenderableMimeType.Image );
            DoTest( "image/bmp", RenderableMimeType.Image );
            DoTest( "image/gif", RenderableMimeType.Image );
            DoTest( "image/vnd.microsoft.icon", RenderableMimeType.Image );
            DoTest( "image/jpeg", RenderableMimeType.Image );
            DoTest( "image/png", RenderableMimeType.Image );
            DoTest( "image/svg+xml", RenderableMimeType.Image );
            DoTest( "image/tiff", RenderableMimeType.Image );
            DoTest( "image/webp", RenderableMimeType.Image );

            DoTest( "video/x-msvideo", RenderableMimeType.Video );
            DoTest( "video/mp4", RenderableMimeType.Video );
            DoTest( "video/mpeg", RenderableMimeType.Video );
            DoTest( "video/ogg", RenderableMimeType.Video );
            DoTest( "video/mp2t", RenderableMimeType.Video );
            DoTest( "video/webm", RenderableMimeType.Video );
            DoTest( "video/3gpp", RenderableMimeType.Video );
            DoTest( "video/3gpp2", RenderableMimeType.Video );

            DoTest( "application/octet-stream", RenderableMimeType.NotRenderable );
        }
    }
}
