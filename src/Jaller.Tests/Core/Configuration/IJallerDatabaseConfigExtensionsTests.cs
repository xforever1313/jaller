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
using LiteDB;

namespace Jaller.Tests.Core.Configuration;

[TestClass]
public sealed class IJallerDatabaseConfigExtensionsTests
{
    // ---------------- Tests ----------------

    /// <summary>
    /// Ensures going from a <see cref="IJallerDatabaseConfig"/> results in 
    /// </summary>
    [TestMethod]
    public void ToConnectionStringTest1()
    {
        // Setup
        var uut = new JallerDatabaseConfig
        {
            AutoRebuild = true,
            AutoUpgradeDb = true,
            DatabaseLocation = null,
            DirectConnection = true,
            EncryptionPassword = null
        };

        // Act
        string str = uut.ToConnectionString();
        var connectionString = new ConnectionString( str );

        // Check
        Assert.IsTrue( connectionString.AutoRebuild );
        Assert.IsTrue( connectionString.Upgrade );
        Assert.AreEqual( ":memory:", connectionString.Filename );
        Assert.AreEqual( ConnectionType.Direct, connectionString.Connection );
        Assert.IsNull( connectionString.Password );
    }

    /// <summary>
    /// Ensures going from a <see cref="IJallerDatabaseConfig"/> results in 
    /// </summary>
    [TestMethod]
    public void ToConnectionStringTest2()
    {
        // Setup
        var uut = new JallerDatabaseConfig
        {
            AutoRebuild = false,
            AutoUpgradeDb = false,
            DatabaseLocation = null,
            DirectConnection = false,
            EncryptionPassword = "somepassword"
        };

        // Act
        string str = uut.ToConnectionString();
        var connectionString = new ConnectionString( str );

        // Check
        Assert.IsFalse( connectionString.AutoRebuild );
        Assert.IsFalse( connectionString.Upgrade );
        Assert.AreEqual( ":memory:", connectionString.Filename );
        Assert.AreEqual( ConnectionType.Shared, connectionString.Connection );
        Assert.AreEqual( "somepassword", connectionString.Password );
    }

    /// <summary>
    /// Ensures going from a <see cref="IJallerDatabaseConfig"/> results in 
    /// </summary>
    [TestMethod]
    public void ToConnectionStringTestWithSemicolonInPassword()
    {
        // Setup
        var uut = new JallerDatabaseConfig
        {
            AutoRebuild = false,
            AutoUpgradeDb = false,
            DatabaseLocation = new FileInfo( "Her;e.db" ),
            DirectConnection = false,
            EncryptionPassword = "somepas;sword"
        };

        // Act
        string str = uut.ToConnectionString();
        var connectionString = new ConnectionString( str );

        // Check
        Assert.IsFalse( connectionString.AutoRebuild );
        Assert.IsFalse( connectionString.Upgrade );
        Assert.AreEqual( uut.DatabaseLocation.FullName, connectionString.Filename );
        Assert.AreEqual( ConnectionType.Shared, connectionString.Connection );
        Assert.AreEqual( "somepas;sword", connectionString.Password );
    }

    /// <summary>
    /// Ensures going from a <see cref="IJallerDatabaseConfig"/> results in 
    /// </summary>
    [TestMethod]
    public void ToConnectionStringTestWithQuoteInPassword()
    {
        // Setup
        var uut = new JallerDatabaseConfig
        {
            AutoRebuild = false,
            AutoUpgradeDb = false,
            DatabaseLocation = new FileInfo( "Her'e.db" ),
            DirectConnection = false,
            EncryptionPassword = "somepas'sword"
        };

        // Act
        string str = uut.ToConnectionString();
        var connectionString = new ConnectionString( str );

        // Check
        Assert.IsFalse( connectionString.AutoRebuild );
        Assert.IsFalse( connectionString.Upgrade );
        Assert.AreEqual( uut.DatabaseLocation.FullName, connectionString.Filename );
        Assert.AreEqual( ConnectionType.Shared, connectionString.Connection );
        Assert.AreEqual( "somepas'sword", connectionString.Password );
    }
}
