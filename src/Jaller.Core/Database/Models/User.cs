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

using System.ComponentModel.DataAnnotations;
using Jaller.Standard.UserManagement;

namespace Jaller.Core.Database.Models
{
    internal sealed record class User
    {
        // ---------------- Fields ----------------

        internal const int MinimumUserNameSize = 3;

        internal const int MaximumUserNameSize = 64;

        // ---------------- Properties ----------------

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength( MaximumUserNameSize, MinimumLength = 3 )]
        public string? UserName { get; set; } = "";

        public UserState UserState { get; set; } = UserState.Active;

        /// <summary>
        /// The method for the user to login as.
        /// </summary>
        public UserAuthenticationMethod AuthenticationMethod { get; set; } = UserAuthenticationMethod.Password;

        /// <summary>
        /// If <see cref="UserAuthenticationMethod"/> is set to <see cref="UserAuthenticationMethod.Password"/>,
        /// then this will be not-null.
        /// </summary>
        public PasswordAuthentication? PasswordAuthentication { get; set; } = null;

        /// <summary>
        /// The files uploaded by this user, if any.
        /// </summary>
        public ICollection<FileUploadInformation>? UploadedFiles { get; set; } = null;

        /// <summary>
        /// The metadata created by this user, if any.
        /// </summary>
        public ICollection<FileUploadInformation>? MetadataCreatedFiles { get; set; } = null;

        /// <summary>
        /// The metadata last modified by this user, if any.
        /// </summary>
        public ICollection<FileUploadInformation>? MetadataLastModifiedFiles { get; set; } = null;
    }
}
