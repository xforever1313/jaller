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
using Jaller.Standard.Configuration;
using Quartz;

namespace Jaller.Server.Tasks;

public sealed class UpdateSearchIndexTask : IJob
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public UpdateSearchIndexTask( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    public async Task Execute( IJobExecutionContext context )
    {
        try
        {
            await Task.Run( () => this.core.Search.Index( context.CancellationToken ) );
        }
        catch( OperationCanceledException )
        {
        }
        catch( Exception e )
        {
            this.core.Log.Error( "Error seen when updating index: " + e.Message );
        }
    }
}

internal static class UpdateSearchIndexTaskExtensions
{
    internal static void AddSearchTask(
        this IServiceCollectionQuartzConfigurator quartzConfig,
        IJallerCore core
    )
    {
        TimeZoneInfo timeZone;
        if( TimeZoneInfo.Local is null )
        {
            core.Log.Warning(
                "Warning! Local Time Zone has not been set. Searching indexing will fire based on UTC.  Please set your Time Zone if you want to use local time."
            );
            timeZone = TimeZoneInfo.Utc;
        }
        else
        {
            timeZone = TimeZoneInfo.Local;
        }

        JobKey key = JobKey.Create( nameof( UpdateSearchIndexTask ) );

        quartzConfig.AddJob<UpdateSearchIndexTask>( key );

        quartzConfig.AddTrigger(
            ( ITriggerConfigurator triggerConfig ) =>
            {
                triggerConfig.WithCronSchedule(
                    core.Config.Search.GetUpdateString( core.Log ),
                    ( CronScheduleBuilder cronBuilder ) =>
                    {
                        // Use the local time zone.)
                        cronBuilder.InTimeZone( timeZone );

                        // If we misfire, don't do anything.  We'll just do it the next go around.
                        cronBuilder.WithMisfireHandlingInstructionDoNothing();
                    }
                ).WithDescription( "Search Index Updater" )
                 .ForJob( key )
                 .StartNow();
            }
        );
    }
}
