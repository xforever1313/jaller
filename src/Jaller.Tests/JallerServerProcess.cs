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

using System.Diagnostics;
using Jaller.Server;

namespace Jaller.Tests
{
    public class JallerServerProcess : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly FileInfo csprojLocation;

        private readonly ManualResetEventSlim programRunningEvent;

        private bool isStarted;

        private readonly Process process;

        // ---------------- Constructor ----------------

        public JallerServerProcess( IJallerTestConfig config )
        {
            this.csprojLocation = new FileInfo(
                Path.Combine(
                    config.TestDirectory.FullName,
                    "..", // Runtime Folder (e.g. net8.0)
                    "..", // Debug
                    "..", // Bin
                    "..", // Jaller.Tests
                    "..", // src
                    "Jaller.Server",
                    "Jaller.Server.csproj"
                )
            );

            this.programRunningEvent = new ManualResetEventSlim();

            this.isStarted = false;

            var processStartInfo = new ProcessStartInfo( "dotnet" )
            {
                Arguments = $@"run --project ""{this.csprojLocation.FullName}"" --no-build --no-restore -- --use_enter_to_exit --config_file=""{config.ConfigFile.FullName}""",
                CreateNoWindow = true,
                ErrorDialog = false,

                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,

                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            this.process = new Process()
            {
                StartInfo = processStartInfo,
            };
        }

        // ---------------- Methods ----------------

        public void Start()
        {
            if( this.isStarted )
            {
                throw new InvalidOperationException( "Process is already started!" );
            }

            this.isStarted = true;
            this.process.Start();
            this.process.Exited += Process_Exited;

            this.process.BeginOutputReadLine();
            this.process.BeginErrorReadLine();
            this.process.OutputDataReceived += Process_OutputDataReceived;
            this.process.ErrorDataReceived += Process_ErrorDataReceived;

            Assert.IsTrue(
                this.programRunningEvent.Wait( new TimeSpan( 0, 0, 15 ) ), "Process failed to start."
            );
        }

        public int Stop()
        {
            if( this.isStarted == false )
            {
                return 0;
            }

            if( this.process.HasExited )
            {
                return this.process.ExitCode;
            }

            bool failedToStop = false;
            this.process.StandardInput.WriteLine( "" );
            if( this.process.WaitForExit( new TimeSpan( 0, 0, 15 ) ) == false )
            {
                this.process.Kill();
                failedToStop = true;
            }

            Assert.IsTrue( this.process.WaitForExit( new TimeSpan( 0, 0, 15 ) ), "Server process did not exit" );
            Assert.IsFalse( failedToStop, "Process had to be killed" );

            return this.process.ExitCode;
        }

        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch
            {
                // Do nothing, we're dispoing anyways.
            }
            finally
            {
                this.process.OutputDataReceived -= Process_OutputDataReceived;
                this.process.ErrorDataReceived -= Process_ErrorDataReceived;
                this.process.Exited -= Process_Exited;
                this.process.Dispose();

                this.programRunningEvent.Dispose();
            }

            GC.SuppressFinalize( this );
        }

        private void Process_OutputDataReceived( object sender, DataReceivedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( e.Data ) == false )
            {
                Console.WriteLine( e.Data );

                if( Program.RunningMessage.Equals( e.Data.Trim() ) )
                {
                    this.programRunningEvent.Set();
                }
            }
        }

        private void Process_ErrorDataReceived( object sender, DataReceivedEventArgs e )
        {
            if( string.IsNullOrWhiteSpace( e.Data ) == false )
            {
                Console.Error.WriteLine( e.Data );
            }
        }

        private void Process_Exited( object? sender, EventArgs e )
        {
            this.programRunningEvent.Set();
        }
    }
}
