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

using Jaller.Standard.Configuration;
using Jaller.Standard.Logging;

namespace Jaller.Tests
{
    public class StandardTestConfig : IJallerTestConfig
    {
        // ---------------- Constructor ----------------

        public StandardTestConfig( string testFixtureName, ushort portNumber )
        {
            string? directory = Path.GetDirectoryName( GetType().Assembly.Location );
            Assert.IsNotNull( directory );

            this.TestFixtureName = testFixtureName;

            this.TestDirectory = new DirectoryInfo(
                Path.Combine( directory, testFixtureName )
            );

            this.ConfigFile = new FileInfo(
                Path.Combine( this.TestDirectory.FullName, "JallerCofig.cs" )
            );

            this.PortNumber = portNumber;
        }

        // ---------------- Properties ----------------

        public string TestFixtureName { get; }

        public DirectoryInfo TestDirectory { get; }

        public FileInfo ConfigFile { get; }

        public ushort PortNumber { get; }

        // ---------------- Methods ----------------

        public virtual string ToJallerConfigFile()
        {
            return
$@"
{nameof( IJallerConfig.Logging )}.{nameof( IJallerLoggingConfig.ConsoleLogLevel )} = {nameof( JallerLogLevel )}.{JallerLogLevel.Verbose};

{nameof( IJallerConfig.Database )}.{nameof( IJallerDatabaseConfig.DatabaseLocation )} = new FileInfo( @""{Path.Combine( this.TestDirectory.FullName, $"{TestFixtureName}.data.ldb" )}"" );

{nameof( IJallerConfig.Search )}.{nameof( IJallerSearchConfig.UpdateIndexOnStartup )} = {true.ToString().ToLower()};
{nameof( IJallerConfig.Search )}.{nameof( IJallerSearchConfig.DatabaseLocation )} = new FileInfo( @""{Path.Combine( this.TestDirectory.FullName, $"{TestFixtureName}.search.ldb" )}"" );

{nameof( IJallerConfig.Users )}.{nameof( IJallerUserConfig.DatabaseLocation )} = new FileInfo( @""{Path.Combine( this.TestDirectory.FullName, $"{TestFixtureName}.users.ldb" )}"" );
{nameof( IJallerConfig.Users )}.{nameof( IJallerUserConfig.AdminEmail )} = ""ipfsadmin@shendrick.net"";
{nameof( IJallerConfig.Users )}.{nameof( IJallerUserConfig.AdminPassword )} = ""Jaller@dm1nPassword"";
{nameof( IJallerConfig.Users )}.{nameof( IJallerUserConfig.AllowAdminUser )} = {true.ToString().ToLower()};
{nameof( IJallerConfig.Users )}.{nameof( IJallerUserConfig.AllowPublicRegistration )} = {true.ToString().ToLower()};

{nameof( IJallerConfig.Web )}.{nameof( IJallerWebConfig.AllowPortsInUrl ) } = {true.ToString().ToLower()};
{nameof( IJallerConfig.Web )}.{nameof( IJallerWebConfig.RewriteDoubleSlashes )} = {false.ToString().ToLower()};
{nameof( IJallerConfig.Web )}.{nameof( IJallerWebConfig.AspNetCoreUrls )} = [""http://localhost:{this.PortNumber}""];
";
        }
    }
}
