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
using Jaller.Core.Configuration;
using LiteDB.Identity;
using LiteDB.Identity.Extensions;
using LiteDB.Identity.Models;
using Microsoft.AspNetCore.Identity;
using SethCS.Exceptions;
using SethCS.Extensions;

namespace Jaller.Server;

public static class UserManager
{
    // ---------------- Methods ----------------

    public static void AddUserManager( this IServiceCollection services, JallerCore core )
    {
        services.AddLiteDBIdentity(
            ( LiteDbIdentityOptions options ) =>
            {
                options.ConnectionString = core.Config.Users.ToConnectionString();
            }
        );

        services.AddDefaultIdentity<LiteDbUser>();
    }

    public static void InitDatabase( this IServiceProvider services, JallerCore core )
    {
        core.Log.Information( "Initializing User Database..." );
        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider.GetService<UserManager<LiteDbUser>>();
        if( userManager is null )
        {
            throw new InvalidOperationException( "Could not acquire user manager." );
        }

        var roleManager = scope.ServiceProvider.GetService<RoleManager<LiteDbRole>>();
        if( roleManager is null )
        {
            throw new InvalidOperationException( "Could not acquire role manager." );
        }

        foreach( Roles role in Enum.GetValues( typeof( Roles ) ) )
        {
            string roleName = role.GetRoleName();
            if( roleManager.RoleExistsAsync( roleName ).Result == false )
            {
                core.Log.Debug( $"Role {roleName} does not exist, adding." );
                roleManager.CreateAsync(
                    new LiteDbRole
                    {
                        Name = roleName
                    }
                );
            }
        }

        if( core.Config.Users.AllowAdminUser )
        {
            AddAdminUser( userManager, core );
        }
        else
        {
            DisableAdminUser( userManager, core );
        }

        core.Log.Information( "Initializing User Database... Done!" );
    }

    private static void AddAdminUser(
        UserManager<LiteDbUser> userManager,
        JallerCore core
    )
    {
        if( string.IsNullOrWhiteSpace( core.Config.Users.AdminEmail ) )
        {
            throw new ValidationException(
                "No admin email was specified, please specify an admin email in the config in order to add an admin user."
            );
        }

        LiteDbUser? user = userManager.FindByNameAsync( core.Config.Users.AdminEmail ).Result;
        if( user is not null )
        {
            core.Log.Debug( "Admin user detected, deleting then re-creating." );
            DisableAdminUser( userManager, core );
        }

        core.Log.Debug( "Adding admin user." );
        user = new LiteDbUser
        {
            Email = core.Config.Users.AdminEmail,
            EmailConfirmed = true,
            UserName = core.Config.Users.AdminEmail,
        };

        IdentityResult addResult = userManager.CreateAsync( user, core.Config.Users.AdminPassword ).Result;
        if( addResult.Succeeded == false )
        {
            core.Log.Error( $"Error creating Admin user:{Environment.NewLine}{addResult.Errors.Select( e => e.Description ).ToListString( "- " )}" );
            return;
        }

        addResult = userManager.AddToRoleAsync( user, Roles.Admin.GetRoleName() ).Result;
        if( addResult.Succeeded )
        {
            core.Log.Debug( "Admin user created!" );
        }
        else
        {
            core.Log.Error( $"Error adding Admin user to admin role:{Environment.NewLine}{addResult.Errors.Select( e => e.Description ).ToListString( "- " )}" );
        }
    }

    private static void DisableAdminUser(
        UserManager<LiteDbUser> userManager,
        JallerCore core
    )
    {
        LiteDbUser? user = userManager.FindByNameAsync( core.Config.Users.AdminEmail ).Result;
        if( user is null )
        {
            core.Log.Debug( "Admin user is disabled, but does not exist.  Doing nothing." );
            return;
        }

        IdentityResult deleteResult = userManager.DeleteAsync( user ).Result;
        if( deleteResult.Succeeded )
        {
            core.Log.Debug( "Existing admin user deleted." );
        }
        else
        {
            core.Log.Error( $"Error deleting Admin user:{Environment.NewLine}{deleteResult.Errors.Select( e => e.Description ).ToListString( "- " )}" );
        }
    }
}
