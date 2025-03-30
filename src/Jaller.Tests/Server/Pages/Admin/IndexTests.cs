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
using Jaller.Server.Pages.Admin;
using Jaller.Tests.Mocks;

namespace Jaller.Tests.Server.Pages.Admin;

[TestClass]
public sealed class IndexTests
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

    private JallerCore Core
    {
        get
        {
            ArgumentNullException.ThrowIfNull( this.core );
            return this.core;
        }
    }

    // ---------------- Tests ----------------

    [TestMethod]
    public void HostRestrictionTest()
    {
        // Setup
        var uut = new IndexModel( this.Core );

        // Act / Check
        CommonAdminUnitTests.DoHostRestrictionTest(
            () => uut.OnGetAsync().Result,
            uut,
            this.Config.Web
        );
    }
}
