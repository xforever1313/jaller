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
using Microsoft.EntityFrameworkCore;

namespace Jaller.Core.Database.Models;

/// <summary>
/// A file that is pinned to IPFS.
/// </summary>
internal sealed record class File
{
    // ---------------- Properties ----------------

    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The original file name of the uploaded file.
    /// Null for unknown.
    /// </summary>
    public string? OriginalFileName { get; set; } = null;

    /// <summary>
    /// How the metadata of the file is viewable.
    /// </summary>
    public FileMetadataPrivacy Metadata { get; set; } = FileMetadataPrivacy.Private;

    /// <summary>
    /// If the file is downloadable to the public or requires a login.
    /// </summary>
    public FileDownloadable Downloadable { get; set; } = FileDownloadable.Private;

    /// <summary>
    /// The content IDs that this file supports.
    /// Null for no content IDs
    /// </summary>
    public ICollection<ContentId>? Cids { get; set; } = null;

    /// <summary>
    /// The upload information of the file, if any.
    /// Null for none.
    /// </summary>
    public FileUploadInformation? UploadInformation { get; set; } = null;

    /// <summary>
    /// The download stats of the file, if any.
    /// Null for none.
    /// </summary>
    public FileDownloadInformation? DownloadInformation { get; set; } = null;

    // ---------------- Methods ----------------

    internal static void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<File>()
            .HasOne( file => file.UploadInformation )
            .WithOne( metadata => metadata.File )
            .HasForeignKey<FileUploadInformation>( metadata => metadata.FileId );

        modelBuilder.Entity<File>()
            .HasOne( file => file.DownloadInformation )
            .WithOne( metadata => metadata.File )
            .HasForeignKey<FileDownloadInformation>( metadata => metadata.FileId );
    }
}
