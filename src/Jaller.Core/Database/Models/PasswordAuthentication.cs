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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaller.Core.Database.Models
{
    internal sealed record class PasswordAuthentication
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The hashed password.
        /// </summary>
        public string? HashedPassword { get; set; }

        /// <summary>
        /// The user's role.
        /// </summary>
        /// <remarks>
        /// This is in this table since this needs to be set by hand,
        /// while something like LDAP would have the user role be defined
        /// in LDAP groups.
        /// </remarks>
        public UserRole UserRole { get; set; } = UserRole.User;

        /// <summary>
        /// Two factor authentication method, if any.
        /// </summary>
        public TwoFactorAuthMethod TwoFactorAuthMethod { get; set; } = TwoFactorAuthMethod.None;

        /// <summary>
        /// The TOTP Key if two-factory-auth is enabled.
        /// </summary>
        public string? TotpKey { get; set; } = null;

        public int UserId { get; set; }

        [ForeignKey( nameof( UserId ) )]
        public User? User { get; set; }
    }
}
