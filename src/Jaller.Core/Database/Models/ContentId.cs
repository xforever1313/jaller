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
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jaller.Core.Database.Models;

internal sealed record class ContentId
{
    // ---------------- Fields ----------------

    /// <remarks>
    /// Seemingly, it appears as though the maximum length
    /// is 64 bytes for V1 hashes.
    /// Source: https://github.com/multiformats/cid/issues/21#issuecomment-395871987
    /// "By default id hashes will have a maxium digest length (and thus content length) of 64 bytes"
    /// This is seemingly true in all cases called out here.
    /// </remarks>
    internal const int MaxCidLength = 64;

    // ---------------- Properties ----------------

    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The V1 Hash of the CID.
    /// </summary>
    [Required]
    [MaxLength( MaxCidLength )]
    public string V1Hash { get; set; } = "";

    public int FileId { get; set; }

    [ForeignKey( nameof( FileId ) )]
    public File? File { get; set; } = null;

    // ---------------- Methods ----------------

    internal static void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<ContentId>()
            .HasOne( conentId => conentId.File )
            .WithMany( file => file.Cids )
            .HasForeignKey( c => c.FileId );
    }
}
