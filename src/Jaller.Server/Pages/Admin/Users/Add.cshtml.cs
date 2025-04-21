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

using System.Net;
using Jaller.Server.Extensions;
using Jaller.Server.Models;
using Jaller.Standard;
using LiteDB.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SethCS.Extensions;

namespace Jaller.Server.Pages.Admin.Users;

public sealed class AddModel : PageModel, IAlert
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    private readonly UserManager<LiteDbUser> userManager;

    // ---------------- Constructor ----------------

    public AddModel( IJallerCore core, UserManager<LiteDbUser> userManager )
    {
        this.core = core;
        this.userManager = userManager;
    }

    // ---------------- Properties ----------------

    public string? UserEmail { get; set; } = null;
    
    public string? Password { get; set; } = null;

    public bool IsEditor { get; set; } = false;

    public bool IsUploader { get; set; } = false;

    public bool IsAdmin { get; set; } = false;

    /// <inheritdoc/>
    [TempData( Key = "AddUserInfoMessage" )]
    public string? InfoMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddUserWarningMessage" )]
    public string? WarningMessage { get; set; }

    /// <inheritdoc/>
    [TempData( Key = "AddUserErrorMessage" )]
    public string? ErrorMessage { get; set; }

    // ---------------- Methods ----------------

    public IActionResult OnGet()
    {
        // So we don't accidentally leak it somehow.
        this.Password = null;
        
        if( this.AllowAdminAccess() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        this.InfoMessage = null;
        this.WarningMessage = null;
        this.ErrorMessage = null;

        if( this.AllowAdminAccess() == false )
        {
            return StatusCode( (int)HttpStatusCode.Forbidden );
        }
        else if( string.IsNullOrWhiteSpace( this.UserEmail ) )
        {
            this.ErrorMessage = "User Email must be specified!";
            return RedirectToPage();
        }
        else if( string.IsNullOrWhiteSpace( this.Password ) )
        {
            this.ErrorMessage = "User password must be specified!";
            return RedirectToPage();
        }

        core.Log.Debug( "Adding new user." );
        var user = new LiteDbUser
        {
            Email = this.UserEmail,
            EmailConfirmed = true,
            UserName = this.UserEmail,
        };

        IdentityResult addResult = await userManager.CreateAsync( user, this.Password );
        if( addResult.Succeeded == false )
        {
            this.ErrorMessage = $"Error creating user:{Environment.NewLine}{addResult.Errors.Select( e => e.Description ).ToListString( "- " )}";
            return RedirectToPage();
        }

        var roleErrors = new List<string>();

        // Confirm user right away.
        addResult = await this.userManager.AddToRoleAsync( user, Roles.User.GetRoleName() );
        if( addResult.Succeeded == false )
        {
            roleErrors.AddRange( addResult.Errors.Select( e => e.Description ) );
        }

        if( this.IsUploader )
        {
            addResult = await this.userManager.AddToRoleAsync( user, Roles.Uploader.GetRoleName() );
            if( addResult.Succeeded == false )
            {
                roleErrors.AddRange( addResult.Errors.Select( e => e.Description ) );
            }
        }

        if( this.IsEditor )
        {
            addResult = await this.userManager.AddToRoleAsync( user, Roles.Editor.GetRoleName() );
            if( addResult.Succeeded == false )
            {
                roleErrors.AddRange( addResult.Errors.Select( e => e.Description ) );
            }
        }

        if( this.IsAdmin )
        {
            addResult = await this.userManager.AddToRoleAsync( user, Roles.Admin.GetRoleName() );
            if( addResult.Succeeded == false )
            {
                roleErrors.AddRange( addResult.Errors.Select( e => e.Description ) );
            }
        }

        if( roleErrors.Any() )
        {
            this.ErrorMessage = $"User created, but could not add to role:{Environment.NewLine}{addResult.Errors.Select( e => e.Description ).ToListString( "- " )}";
            return RedirectToPage();
        }

        return RedirectToPage();
    }
}
