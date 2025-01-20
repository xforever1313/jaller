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

namespace Jaller.Tests.Core
{
    [TestClass]
    public sealed class CidTests
    {
        // ---------------- Tests ----------------

        /// <summary>
        /// Given various IPFS version 0 hashes,
        /// does the hash convert between version 0 and version
        /// 1 correctly?
        /// </summary>
        [TestMethod]
        public void CidVersion0ParseTest()
        {
            DoParseV0Test(
                "Qmaobf4QpYonZL4dGDhoc68ZQ7SfCwRdcC9fU9TJyQnBpY",
                "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u" 
            );

            DoParseV0Test(
                "QmVXrfQGSbErw7nNGYD2v4VU6dUvdfJPLJaAyzwuFeKSWH",
                "bafybeidk4c3i3opkvz4unqb6x7ziubdxf7shicazj4zauqfzlplcfdxzay"
            );

            DoParseV0Test(
                "QmQXqxzncc26BeKiYXvxDQ283a9m6XJomWcq6kZGo9jtvA",
                "bafybeibasv62htrvcazopuxuegadvdh7apluyrafxumblulilznjdukcy4"
            );
        }

        /// <summary>
        /// Given various IPFS version 1 hashes,
        /// does the hash convert between version 1 and version
        /// 0 correctly?
        /// </summary>
        [TestMethod]
        public void CidVersion1ParseTest()
        {
            DoParseV1Test(
                "Qmaobf4QpYonZL4dGDhoc68ZQ7SfCwRdcC9fU9TJyQnBpY",
                "bafybeifzgn4th5udmc4u6hnv4b4xeaommqn64g763ifwbc3pa6ihemfx4u"
            );

            DoParseV1Test(
                "QmVXrfQGSbErw7nNGYD2v4VU6dUvdfJPLJaAyzwuFeKSWH",
                "bafybeidk4c3i3opkvz4unqb6x7ziubdxf7shicazj4zauqfzlplcfdxzay"
            );

            DoParseV1Test(
                "QmQXqxzncc26BeKiYXvxDQ283a9m6XJomWcq6kZGo9jtvA",
                "bafybeibasv62htrvcazopuxuegadvdh7apluyrafxumblulilznjdukcy4"
            );
        }

        // ---------------- Test Helpers ----------------

        private void DoParseV0Test( string expectedV0, string expectedV1 )
        {
            DoParseTest( expectedV0, expectedV0, expectedV1 );
        }

        private void DoParseV1Test( string expectedV0, string expectedV1 )
        {
            DoParseTest( expectedV1, expectedV0, expectedV1 );
        }

        private void DoParseTest( string uutHash, string expectedV0, string expectedV1 )
        {
            var cid = Cid.Parse( uutHash );
            Assert.AreEqual( expectedV0, cid.Version0Cid );
            Assert.AreEqual( expectedV1, cid.Version1Cid );
        }
    }
}
