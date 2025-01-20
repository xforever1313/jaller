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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaller.Standard.UserManagement;

/// <summary>
/// The way a user is expected to login as.
/// </summary>
public enum UserAuthenticationMethod : byte
{
    Password = 0,

    /// <summary>
    /// The user has been deleted.
    /// </summary>
    Deleted = 255
}

/// <summary>
/// The role of a particular user.
/// </summary>
public enum UserRole : byte
{
    /// <summary>
    /// The user can only login and download files.
    /// They can not modify metadata or upload any files.
    /// </summary>
    User = 0,

    /// <summary>
    /// The user can add, edit, or delete metadata,
    /// but can not upload files.
    /// </summary>
    Editor = 10,

    /// <summary>
    /// The user can upload files and edit metadata.
    /// They can not perform admin duties, however.
    /// </summary>
    Uploader = 20,

    /// <summary>
    /// The user can add, remove, or modify users, in addition
    /// to all the responsibilies of lower roles.
    /// </summary>
    Moderator = 100,

    /// <summary>
    /// The user has full admin access to the application.
    /// </summary>
    Admin = 255,
}

public enum TwoFactorAuthMethod
{
    None = 0,

    Totp = 1
}

public enum UserState
{
    Active = 0,

    Deleted = 255
}
