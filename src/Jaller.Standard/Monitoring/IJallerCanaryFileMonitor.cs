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

namespace Jaller.Standard.Monitoring;

/// <summary>
/// Monitors <see cref="Configuration.IJallerMonitoringConfig.CanaryFiles"/> and reports if any files
/// are not readable.
/// </summary>
public interface IJallerCanaryFileMonitor
{
    // ---------------- Methods ----------------

    /// <summary>
    /// Gets the files that are missing since the last time the cache was refreshed.
    /// This returns a copy of the cache to avoid multi-threading issues.
    /// Generating a copy of hte cache is thread-safe.
    /// </summary>
    IList<FileInfo> GetMissingFiles();

    /// <summary>
    /// Checks for any files that are missing and updates <see cref="MissingFiles"/>.
    /// Any files that are missing are added to <see cref="MissingFiles"/>, while any files in
    /// <see cref="MissingFiles"/> that are now back are removed.
    /// 
    /// This method does NOT log anything.
    /// </summary>
    void Refresh( CancellationToken cancelToken );

    /// <summary>
    /// Checks for any files that are missing and updates <see cref="MissingFiles"/>.
    /// Any files that are missing are added to <see cref="MissingFiles"/>, while any files in
    /// <see cref="MissingFiles"/> that are now back are removed.  Any changes to the list
    /// are logged in <see cref="JallerLogLevel.Fatal"/>, as a missing canary file is usually bad.
    /// </summary>
    void RefreshAndLogMissingFiles( CancellationToken cancelToken );
}

public static class IJallerCanaryFileMonitorExtensions
{
    /// <summary>
    /// Checks for any files that are missing and updates <see cref="MissingFiles"/>.
    /// Any files that are missing are added to <see cref="MissingFiles"/>, while any files in
    /// <see cref="MissingFiles"/> that are now back are removed.
    /// 
    /// This method does NOT log anything.
    /// </summary>
    public static void Refresh( this IJallerCanaryFileMonitor monitor )
    {
        monitor.Refresh( CancellationToken.None );
    }

    /// <summary>
    /// Checks for any files that are missing and updates <see cref="MissingFiles"/>.
    /// Any files that are missing are added to <see cref="MissingFiles"/>, while any files in
    /// <see cref="MissingFiles"/> that are now back are removed.  Any changes to the list
    /// are logged in <see cref="JallerLogLevel.Fatal"/>, as a missing canary file is usually bad.
    /// </summary>
    public static void RefreshAndLogMissingFiles( this IJallerCanaryFileMonitor monitor )
    {
        monitor.RefreshAndLogMissingFiles( CancellationToken.None );
    }
}
