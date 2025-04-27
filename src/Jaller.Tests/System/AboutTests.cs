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

namespace Jaller.Tests.System;

/// <summary>
/// Tests to ensure we can query all the about pages.
/// </summary>
[TestClass]
[DoNotParallelize]
public sealed class AboutTests
{
    // ---------------- Fields ----------------

    private static JallerTestHarness? harness;

    // ---------------- Setup / Teardown ----------------

    // No shared state between these tests; they're all read-only tests anyways.
    // It is safe to use fixture-level stuff.

    [ClassInitialize]
    public static void TestFixtureSetup( TestContext context )
    {
        harness = new JallerTestHarness( new StandardTestConfig( typeof( AboutTests ).Name, 7000 ) );
        harness.DoTestSetup();
    }

    [ClassCleanup]
    public static void TestFixtureTearDown()
    {
        harness?.DoTestTeardown();
    }

    [TestInitialize]
    public void TestSetup()
    {
    }

    [TestCleanup]
    public void TestTeardown()
    {
    }

    // ---------------- Properties ----------------

    public static JallerTestHarness Harness
    {
        get
        {
            Assert.IsNotNull( harness );
            return harness;
        }
    }

    // ---------------- Tests ----------------

    [TestMethod]
    public void TestIndex()
    {
        // Act
        HttpResponseMessage message = Harness.Client.GetAsync( "/About/" ).Result;

        // Check
        Assert.AreEqual( HttpStatusCode.OK, message.StatusCode );
    }

    [TestMethod]
    public void TestLicense()
    {
        // Act
        HttpResponseMessage message = Harness.Client.GetAsync( "/About/License" ).Result;

        // Check
        Assert.AreEqual( HttpStatusCode.OK, message.StatusCode );
    }

    [TestMethod]
    public void TestCredits()
    {
        // Act
        HttpResponseMessage message = Harness.Client.GetAsync( "/About/Credits" ).Result;

        // Check
        Assert.AreEqual( HttpStatusCode.OK, message.StatusCode );
    }
}
