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

using Jaller.Core.Configuration;
using Jaller.Server.Pages.Admin;
using Jaller.Standard;
using Moq;

namespace Jaller.Tests.Pages.Admin;

[TestClass]
public sealed class IndexTests
{
    // ---------------- Fields ----------------

    private Mock<IJallerCore>? core;

    // ---------------- Setup / Teardown ----------------

    [TestInitialize]
    public void TestSetup()
    {
        this.core = new Mock<IJallerCore>( MockBehavior.Strict );
    }

    [TestCleanup]
    public void TestTeardown()
    {
    }

    // ---------------- Properties ----------------

    private Mock<IJallerCore> Core
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
        var config = new JallerConfig();
        this.Core.Setup( m => m.Config ).Returns( config );

        var uut = new IndexModel( this.Core.Object );

        // Act / Check
        CommonAdminUnitTests.DoHostRestrictionTest(
            () => uut.OnGetAsync().Result,
            uut,
            config.Web
        );
    }
}
