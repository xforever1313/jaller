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

namespace Jaller.Tests
{
    public class JallerTestHarness
    {
        // ---------------- Fields ----------------

        private JallerServerProcess? serverProcess;
        
        private HttpClient? client;

        // ---------------- Constructor ----------------

        public JallerTestHarness( IJallerTestConfig configuration )
        {
            this.TestConfig = configuration;
        }

        // ---------------- Setup / Teardown ----------------

        public void DoTestSetup()
        {
            if( this.TestDirectory.Exists )
            {
                this.TestDirectory.Delete( true );
            }

            this.TestDirectory.Create();

            File.WriteAllText(
                this.TestConfig.ConfigFile.FullName,
                this.TestConfig.ToJallerConfigFile()
            );

            this.client = new HttpClient
            {
                BaseAddress = new Uri( $"http://localhost:{this.TestConfig.PortNumber}" )
            };

            this.serverProcess = new JallerServerProcess( this.TestConfig );
            this.serverProcess.Start();
        }

        public void DoTestTeardown()
        {
            int? exitCode = null;
            if( this.serverProcess is not null )
            {
                exitCode = this.serverProcess.Stop();
                this.serverProcess.Dispose();
            }

            this.client?.Dispose();

            if( this.TestDirectory.Exists )
            {
                this.TestDirectory.Delete( true );
            }

            if( exitCode is not null )
            {
                Assert.AreEqual( 0, exitCode.Value, "Server process did not exit cleanly." );
            }
        }

        // ---------------- Properties ----------------

        public IJallerTestConfig TestConfig { get; }

        public DirectoryInfo TestDirectory => this.TestConfig.TestDirectory;

        public HttpClient Client
        {
            get
            {
                Assert.IsNotNull(
                    this.client,
                    $"Client is null, {nameof( DoTestSetup )} was most likely not called."
                );

                return this.client;
            }
        }
    }
}
