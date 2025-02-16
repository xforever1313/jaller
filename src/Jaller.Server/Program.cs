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
using Jaller.Standard.Configuration;
using Mono.Options;
using Prometheus;
using SethCS.Extensions;

namespace Jaller.Server
{
    public class Program
    {
        // ---------------- Fields ----------------

        internal static readonly string RunningMessage = "Press Enter to Exit...";

        private static Serilog.ILogger? log = null;

        // ---------------- Methods ----------------

        public static int Main( string[] args )
        {
            try
            {
                var options = new ArgumentParser( args );
                int? exitCode = options.Execute();
                if( exitCode is not null )
                {
                    return exitCode.Value;
                }

                IJallerConfig? config = GetConfig( options, out exitCode );
                if( config is null )
                {
                    if( exitCode is not null )
                    {
                        return exitCode.Value;
                    }
                    else
                    {
                        throw new Exception( "Failed to generate configuration, but no exit code was specified." );
                    }
                }

                if( options.CheckConfigOnly )
                {
                    // If we have a config object, everything must have compiled.
                    Console.WriteLine( "Bot configuration is valid!" );
                    return 0;
                }

                log = HostingExtensions.CreateLog( config, OnTelegramFailure );

                var builder = WebApplication.CreateBuilder( args );

                // Add services to the container.
                builder.Services.AddMvc();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if( app.Environment.IsDevelopment() )
                {
                    app.UseWebAssemblyDebugging();
                }
                else
                {
                    app.UseExceptionHandler( "/Error" );
                }

                app.UseDefaultFiles();
                app.UseStaticFiles();

                app.UseRouting();

                if( config.Web.EnableMetrics )
                {
                    // Per https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0014?view=aspnetcore-8.0:
                    // Warnings from this rule can be suppressed if
                    // the target UseEndpoints invocation is invoked without
                    // any mappings as a strategy to organize middleware ordering.
                    #pragma warning disable ASP0014 // Suggest using top level route registrations
                    app.UseEndpoints(
                        endpoints =>
                        {
                            endpoints.MapMetrics( "/Metrics" );
                        }
                    );
                    #pragma warning restore ASP0014 // Suggest using top level route registrations
                }

                // Unsure what we're doing about this yet...
                //app.UseAuthorization();

                app.UseBlazorFrameworkFiles();
#if false
                app.MapRazorComponents<App>()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies( typeof( Jaller.Client._Imports ).Assembly );
#endif
                if( options.UseEnterToExit )
                {
                    Console.WriteLine( RunningMessage );
                    Console.ReadLine();
                }
                else
                {
                    app.Run();
                }

                log?.Information( "Application Exiting" );
                Console.WriteLine( "Application Exiting" );
            }
            catch( OptionException e )
            {
                Console.Error.WriteLine( "Error parsing options:" );
                Console.Error.WriteLine( e.ToString() );
                return 3;
            }
            catch( Exception e )
            {
                if( log is null )
                {
                    Console.Error.WriteLine( "FATAL ERROR:" );
                    Console.Error.WriteLine( e.ToString() );
                }
                else
                {
                    log.Fatal( "FATAL ERROR:" + Environment.NewLine + e );
                }
                return 100;
            }

            return 0;
        }


        private static IJallerConfig? GetConfig( ArgumentParser options, out int? exitCode )
        {
            FileInfo? configFile = options.GetConfigFilePath();

            if( configFile is null )
            {
                configFile = new FileInfo(
                    Path.Combine(
                        JallerConfig.DefaultPersistenceDirectory.FullName,
                        "JallerConfig.cs"
                    )
                );

                if( configFile.Exists == false )
                {
                    DirectoryInfo? defaultDirectory = configFile.Directory;
                    if( ( defaultDirectory is not null ) && defaultDirectory.Exists == false )
                    {
                        Directory.CreateDirectory( defaultDirectory.FullName );
                    }
                }

                if( configFile.Exists == false )
                {
                    WriteDefaultConfig( configFile );
                    Console.WriteLine( "No configuration file has been specifed, and a default one does not exist." );
                    Console.WriteLine( $"A default configuration file has been created at: {configFile.FullName}" );
                    Console.WriteLine( "Please fill out the configuration file, and re-run this program." );
                    Console.WriteLine();
                    Console.WriteLine( "See https://github.com/xforever1313/jaller/blob/main/Docker/jaller/JallerConfig.cs for an example on how to configure this program." );
                    exitCode = 2;
                    return null;
                }
            }

            Console.WriteLine( $"Using config file: {configFile.FullName}" );
            if( configFile.Exists == false )
            {
                Console.WriteLine( "Config file does not exist." );
                exitCode = 4;
                return null;
            }

            var configCompiler = new ConfigCompiler( configFile );
            configCompiler.Preprocess();
            
            IJallerConfig config = configCompiler.Compile();
            List<string> errors = config.TryValidate();
            if( errors.Any() )
            {
                Console.WriteLine( "Bot is misconfigured." );
                Console.WriteLine( errors.ToListString( " - " ) );
                exitCode = 1;
                return null;
            }

            exitCode = null;
            return config;
        }

        private static void WriteDefaultConfig( FileInfo fileLocation )
        {
            string fileContents =
$@"
// TODO
";

            File.WriteAllText( fileLocation.FullName, fileContents );
        }

        private static void OnTelegramFailure( Exception e )
        {
            log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
        }
    }
}
