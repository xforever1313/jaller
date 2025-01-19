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

/// <summary>
/// A folder folks can put files inside of.
/// </summary>
internal record class Folder
{
    // ---------------- Fields ----------------

    internal const int nameSize = 255;

    // ---------------- Properties ----------------

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength( nameSize )]
    public string Name { get; set; } = "New Folder";

    /// <summary>
    /// The slug for a URL for the folder.
    /// If null, it uses the default of replacing white space
    /// or special characters with '_'.
    /// </summary>
    [MaxLength( nameSize )]
    public string? Slug { get; set; }

    public int? ParentFolderId { get; set; }

    /// <summary>
    /// The parent folder, if any.  Null means no
    /// parent folder (the folder is at the root).
    /// </summary>
    [ForeignKey( nameof( ParentFolderId ) )]
    public Folder? ParentFolder { get; set; }

    public ICollection<Folder>? ChildFolders { get; set; }

    // ---------------- Methods ----------------

    internal static void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<Folder>()
            .HasOne( f => f.ParentFolder )
            .WithMany( f => f.ChildFolders )
            .HasForeignKey( f => f.ParentFolderId )
            .OnDelete( DeleteBehavior.Restrict ); // Prevents cascading deletes
    }
}
