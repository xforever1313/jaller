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

using Jaller.Server.Extensions;
using Jaller.Standard;
using Quartz;

namespace Jaller.Server.Tasks;

public sealed class CanaryFileCheckTask : IJob
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public CanaryFileCheckTask( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    public async Task Execute( IJobExecutionContext context )
    {
        try
        {
            this.core.Log.Verbose( "Checking for canary files" );
            await Task.Run( () => this.core.CanaryFileMonitor.RefreshAndLogMissingFiles( context.CancellationToken ) );
        }
        catch( OperationCanceledException )
        {
        }
        catch( Exception e )
        {
            this.core.Log.Error( "Error seen when checking for canay files: " + e.Message );
        }
    }
}

internal static class CanaryFileCheckTaskExtensions
{
    internal static void AddCanaryCheckTask(
        this IServiceCollectionQuartzConfigurator quartzConfig,
        IJallerCore core
    )
    {
        if( core.Config.Monitoring.CanaryFiles is null )
        {
            core.Log.Verbose( "Canary files setting null, will not monitor canary files." );
            return;
        }
        else if( core.Config.Monitoring.CanaryFiles.Any() == false )
        {
            core.Log.Verbose( "Canary files setting empty, will not monitor canary files." );
            return;
        }

        TimeZoneInfo timeZone;
        if( TimeZoneInfo.Local is null )
        {
            timeZone = TimeZoneInfo.Utc;
        }
        else
        {
            timeZone = TimeZoneInfo.Local;
        }

        JobKey key = JobKey.Create( nameof( CanaryFileCheckTask ) );

        quartzConfig.AddJob<CanaryFileCheckTask>( key );

        quartzConfig.AddTrigger(
            ( ITriggerConfigurator triggerConfig ) =>
            {
                triggerConfig.WithCronSchedule(
                    core.Config.Monitoring.CanaryCheckRate,
                    ( CronScheduleBuilder cronBuilder ) =>
                    {
                        // Use the local time zone.)
                        cronBuilder.InTimeZone( timeZone );

                        // If we misfire, don't do anything.  We'll just do it the next go around.
                        cronBuilder.WithMisfireHandlingInstructionDoNothing();
                    }
                ).WithDescription( "Canary File Checker" )
                 .ForJob( key )
                 .StartNow();
            }
        );
    }
}
