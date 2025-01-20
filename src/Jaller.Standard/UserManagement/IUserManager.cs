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

namespace Jaller.Standard.UserManagement;

public interface IUserManager
{
    /// <summary>
    /// Configures the given user.
    /// If <see cref="User.Id"/> is 0, a new user is created.
    /// Otherwise the user is modified.
    /// </summary>
    /// <returns>
    /// The ID of the user.
    /// </returns>
    Task<int> ConfigureUserAsync( User user );

    /// <summary>
    /// Gets a user by the given ID.
    /// </summary>
    /// <returns>
    /// Null if user can not be found.
    /// </returns>
    Task<User?> TryGetUserByIdAsync( int id, UserQueryOptions queryOptions );

    /// <summary>
    /// Gets a user by their user name.
    /// </summary>
    /// <returns>Null if the user can not be found.</returns>
    Task<User?> TryGetUserByNameAsync( string userName, UserQueryOptions queryOptions );

    /// <summary>
    /// Deletes the user by its id.
    /// </summary>
    Task DeleteUserAsync( int userId );
}

public static class IUserManagerExtensions
{
    public static void DeleteUser( this IUserManager userManager, User user )
    {
        userManager.DeleteUserAsync( user.Id );
    }
}
