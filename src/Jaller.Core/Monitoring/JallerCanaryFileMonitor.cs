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
using Jaller.Standard.Monitoring;

namespace Jaller.Core.Monitoring;

internal sealed class JallerCanaryFileMonitor : IJallerCanaryFileMonitor
{
    // ---------------- Fields ----------------

    private readonly IJallerMonitoringConfig config;

    private readonly IJallerLogger log;

    private readonly HashSet<FileInfo> missingFiles;

    // ---------------- Constructor ----------------

    public JallerCanaryFileMonitor( IJallerMonitoringConfig config, IJallerLogger log )
    {
        this.config = config;
        this.log = log;

        this.missingFiles = new HashSet<FileInfo>();
    }

    // ---------------- Methods ----------------

    /// <inheritdoc/>
    public IList<FileInfo> GetMissingFiles()
    {
        lock( this.missingFiles )
        {
            return this.missingFiles.ToList();
        }
    }

    /// <inheritdoc/>
    public void Refresh( CancellationToken cancelToken )
    {
        RefreshAndLogMissingFiles( null, cancelToken );
    }

    /// <inheritdoc/>
    public void RefreshAndLogMissingFiles( CancellationToken cancelToken )
    {
        RefreshAndLogMissingFiles( this.log, cancelToken );
    }

    private void RefreshAndLogMissingFiles( IJallerLogger? log, CancellationToken cancelToken )
    {
        if( this.config.CanaryFiles is null )
        {
            return;
        }
        else if( this.config.CanaryFiles.Any() == false )
        {
            return;
        }

        var badFiles = new List<FileInfo>();

        foreach( FileInfo canary in this.config.CanaryFiles )
        {
            cancelToken.ThrowIfCancellationRequested();

            try
            {
                canary.Refresh();
                if( canary.Exists == false )
                {
                    badFiles.Add( canary );
                }
            }
            catch( IOException e )
            {
                this.log.Error( $"Exception caught when checking for canary file '{canary.FullName}': {e}" );
                badFiles.Add( canary );
            }
        }

        lock( this.missingFiles )
        {
            foreach( FileInfo badFile in badFiles )
            {
                if( this.missingFiles.Contains( badFile ) == false )
                {
                    log?.Fatal( $"**CANARY DEAD** A canary file is missing or could not be read.  A drive may have gone down.  File location: {badFile.FullName}" );
                    this.missingFiles.Add( badFile );
                }
            }

            foreach( FileInfo cachedFile in this.missingFiles.ToArray() )
            {
                if( badFiles.Contains( cachedFile ) == false )
                {
                    log?.Fatal( $"**RESOLVED** A canary file has reappeared after disappearing.  A drive may have come back.  File location: {cachedFile.FullName}" );
                    this.missingFiles.Remove( cachedFile );
                }
            }
        }
    }
}

