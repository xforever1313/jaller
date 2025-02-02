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

using Jaller.Standard;
using Jaller.Standard.UserManagement;

namespace Jaller.Core.UserManagement;

public sealed class UserManager : IUserManager
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    // ---------------- Constructor ----------------

    public UserManager( IJallerCore core )
    {
        this.core = core;
    }

    // ---------------- Methods ----------------

    public async Task<int> ConfigureUserAsync( User user )
    {
        await Task.Delay( 0 );
        throw new NotImplementedException();
#if false
        using var context = new JallerDbContext( this.core );

        bool add;
        Database.Models.User? dbUser = await context.Users.FirstOrDefaultAsync( dbUser => dbUser.Id == user.Id );
        if( dbUser is null )
        {
            add = true;
            dbUser = new Database.Models.User();
        }
        else
        {
            if( dbUser.UserState == UserState.Deleted )
            {
                throw new UserDeletedException( user.Id );
            }

            add = false;
        }

        dbUser = dbUser with
        {
            UserName = user.UserName,
            UserState = UserState.Active
        };

        if( add )
        {
            context.Users.Add( dbUser );
        }
        else
        {
            context.Users.Update( dbUser );
        }

        await context.SaveChangesAsync();

        return dbUser.Id;
#endif
    }

    public async Task<User?> TryGetUserByIdAsync( int id, UserQueryOptions queryOptions )
    {
        await Task.Delay( 0 );
        throw new NotImplementedException();
    }

    public async Task<User?> TryGetUserByNameAsync( string userName, UserQueryOptions queryOptions )
    {
        await Task.Delay( 0 );
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync( int userId )
    {
        await Task.Delay( 0 );
        throw new NotImplementedException();
#if false
        using var context = new JallerDbContext( this.core );

        Database.Models.User? user = context.Users.FirstOrDefault( user => user.Id == userId );
        if( user is null )
        {
            throw new UserNotFoundException( userId );
        }

        user = user with
        {
            AuthenticationMethod = UserAuthenticationMethod.Deleted,
            UserState = UserState.Deleted,
            UserName = $"Deleted User {user.Id}",
            PasswordAuthentication = null
        };

        context.Users.Update( user );
        await context.SaveChangesAsync();
#endif
    }
}
