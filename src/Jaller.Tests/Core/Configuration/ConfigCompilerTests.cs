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
using Jaller.Server;
using Jaller.Server.Configuration;
using Jaller.Standard.Configuration;

namespace Jaller.Tests.Core.Configuration;

[TestClass]
public sealed class ConfigCompilerTests
{
    // ---------------- Tests ----------------

    /// <summary>
    /// Ensures the sample config file included in the docker folder
    /// compiles with no issue.
    /// </summary>
    [TestMethod]
    public void SampleConfigCompileTest()
    {
        // Setup
        string sourceCode = Resources.GetDefaultConfiguration();

        var uut = new ConfigCompiler( sourceCode );

        // Act
        uut.Preprocess();
        IJallerConfig config = uut.Compile();
        IList<string> validationErrors = config.TryValidate( new CronStringValidator() );

        // Check
        Assert.IsNotNull( config );
        Assert.IsNotNull( config.Database );
        Assert.IsNotNull( config.Ipfs );
        Assert.IsNotNull( config.Logging );
        Assert.IsNotNull( config.Users );
        Assert.IsNotNull( config.Web );

        // Sample config should not include any validation errors.
        Assert.AreEqual( 0, validationErrors.Count );
    }
}
