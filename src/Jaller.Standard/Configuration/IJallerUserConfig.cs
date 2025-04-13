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

namespace Jaller.Standard.Configuration;

public interface IJallerUserConfig : IJallerDatabaseConfig
{
    /// <summary>
    /// Allows for anyone to create their own account if set to true.
    /// Defaulted to false.
    /// </summary>
    bool AllowPublicRegistration { get; }

    /// <summary>
    /// Set to true to allow a default "admin" user in.
    /// Set to false to disable the default "admin" user.
    /// </summary>
    /// <remarks>
    /// Once a user is created with the "admin" role, this should
    /// probably be set to false.  This should only be set to true
    /// if all admins lost their password or something to that effect.
    /// </remarks>
    bool AllowAdminUser { get; }

    /// <summary>
    /// The admin's email.  This must be specified if <see cref="AllowAdminUser"/> is set to true.
    /// Ignore if <see cref="AllowAdminUser"/> is false.
    /// </summary>
    string AdminEmail { get; }

    /// <summary>
    /// The password to login as for the admin user.
    /// This is ignored if <see cref="AllowAdminUser"/> is false.
    /// </summary>
    string AdminPassword { get; }
}
