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
using Jaller.Core.Configuration;

namespace Jaller.Tests.System;

/// <summary>
/// Tests to ensure we can query all the about pages.
/// </summary>
//[TestClass]
//[DoNotParallelize]
public sealed class AboutTests
{
    // ---------------- Fields ----------------

    private TestJallerServer? server;

    private Task? runTask;

    private static HttpClient? client;

    // ---------------- Setup / Teardown ----------------

    [ClassInitialize]
    public static void FixtureSetup( TestContext context )
    {
        client = new HttpClient();
    }

    [ClassCleanup]
    public static void FixtureTeardown()
    {
        client?.Dispose();
    }

    [TestInitialize]
    public void TestSetup()
    {
        var config = new JallerConfig
        {

        }.UseInMemoryDatabase();

        this.server = new TestJallerServer( config );
        this.runTask = this.server.RunAsync();
    }

    [TestCleanup]
    public void TestTeardown()
    {
        this.server?.Stop();
        this.runTask?.Wait();
        this.server?.Dispose();
    }

    // ---------------- Properties ----------------

    public static HttpClient Client
    {
        get
        {
            Assert.IsNotNull( client );
            return client;
        }
    }

    // ---------------- Tests ----------------

    [TestMethod]
    public void TestLicense()
    {
        // Act
        HttpResponseMessage message = Client.GetAsync( "http://localhost:5000/About/License" ).Result;

        // Check
        Assert.AreEqual( HttpStatusCode.OK, message.StatusCode );
    }

    [TestMethod]
    public void TestCredits()
    {
        // Act
        HttpResponseMessage message = Client.GetAsync( "http://localhost:5000/About/Credits" ).Result;

        // Check
        Assert.AreEqual( HttpStatusCode.OK, message.StatusCode );
    }
}