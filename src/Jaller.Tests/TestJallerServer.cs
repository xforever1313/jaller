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

using Jaller.Server;
using Jaller.Standard.Configuration;

namespace Jaller.Tests;

public class TestJallerServer : JallerServer
{
    // ---------------- Fields ----------------

    private readonly ManualResetEventSlim exitEvent;

    private readonly ManualResetEventSlim setupCompleted;

    // ---------------- Constructor ----------------

    public TestJallerServer( IJallerConfig config ) :
        this( config, new ManualResetEventSlim( false ) )
    {
    }

    private TestJallerServer( IJallerConfig config, ManualResetEventSlim exitEvent ) :
        base( config, () => exitEvent.Wait() )
    {
        this.exitEvent = exitEvent;
        this.setupCompleted = new ManualResetEventSlim( false );
    }

    // ---------------- Methods ----------------

    public Task RunAsync()
    {
        Task task = Task.Run( () => this.Run() );
        if( this.setupCompleted.Wait( new TimeSpan( 0, 1, 0 ) ) == false )
        {
            Assert.Fail( "Failed to start up Jaller in under a minute." );
        }

        Thread.Sleep( new TimeSpan( 0, 0, 7 ) );
        return task;
    }

    /// <summary>
    /// Stop running the server.
    /// </summary>
    public void Stop()
    {
        this.exitEvent.Set();
    }

    protected override void Dispose( bool fromDispose )
    {
        this.exitEvent.Set();
        this.exitEvent.Dispose();

        this.setupCompleted.Dispose();
        base.Dispose( fromDispose );
    }

    protected override void SetupCompleted()
    {
        this.setupCompleted.Set();
    }
}