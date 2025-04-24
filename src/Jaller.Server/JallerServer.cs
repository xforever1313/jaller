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

using Jaller.Core;
using Jaller.Server.Logging;
using Jaller.Standard;
using Jaller.Standard.Configuration;
using Prometheus;

namespace Jaller.Server;

public class JallerServer : IDisposable
{
    // ---------------- Fields ----------------

    private readonly IJallerConfig config;

    private readonly Action? waitAction;

    // ---------------- Constructor ----------------

    public JallerServer( IJallerConfig config, Action? waitAction )
    {
        this.config = config;
        this.Log = HostingExtensions.CreateLog( config, OnTelegramFailure );
        this.waitAction = waitAction;
    }

    ~JallerServer()
    {
        Dispose( false );
    }

    // ---------------- Properties ----------------

    public Serilog.ILogger Log { get; }

    // ---------------- Methods ----------------

    public void Run()
    {
        Run( Array.Empty<string>() );
    }

    public void Run( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );

        using var core = new JallerCore( config, new JallerLogger( Log ) );
        core.Init();

        // Add services to the container.
        builder.Services.AddUserManager( core );

        builder.Services.AddControllers().AddXmlSerializerFormatters();
        builder.Services.AddMvc();
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton<IJallerCore>( core );

        var app = builder.Build();

        if( config.Web.AllowPortsInUrl == false )
        {
            app.Use(
                ( HttpContext context, RequestDelegate next ) =>
                {
                    int? port = context.Request.Host.Port;
                    if( port is not null )
                    {
                        // Kill the connection,
                        // and stop all processing.
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Connection.RequestClose();
                        return Task.CompletedTask;
                    }

                    return next( context );
                }
            );
        }
        
        if( config.Web.RewriteDoubleSlashes )
        {
            app.Use( ( context, next ) =>
            {
                string? value = context.Request.Path.Value;
                if( ( value is not null ) && value.StartsWith( "//" ) )
                {
                    context.Request.Path = new PathString( value.Replace( "//", "/" ) );
                }
                return next();
            } );
        }

        if( config.Users.AllowPublicRegistration == false )
        {
            app.Use(
                async( context, next ) =>
                {
                    string? path = context.Request.Path.Value?.ToLower();
                    if( string.IsNullOrWhiteSpace( path ) == false )
                    {
                        if( path.Contains("/identity/account/register") || path.EndsWith( "/register" ) )
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            await context.Response.WriteAsync(
                                "Registration is disabled."
                            );
                            return;
                        }
                    }

                    await next();
                }
            );
        }

        app.MapControllers();

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
        app.UseStatusCodePagesWithReExecute( "/Errors/{0}" );

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

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

        app.MapRazorPages();

        app.UseBlazorFrameworkFiles();

        app.Services.InitDatabase( core );

        SetupCompleted();

        if( this.waitAction is not null )
        {
            this.waitAction.Invoke();
            app.StopAsync().Wait();
        }
        else
        {
            app.Run();
        }
    }

    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool fromDispose )
    {
    }

    protected virtual void SetupCompleted()
    {
    }

    private void OnTelegramFailure( Exception e )
    {
        this.Log.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
    }
}