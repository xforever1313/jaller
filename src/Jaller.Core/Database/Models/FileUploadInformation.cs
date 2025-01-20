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

namespace Jaller.Core.Database.Models;

internal sealed record class FileUploadInformation
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The timestamp of when the file was uploaded,
    /// if any.
    /// </summary>
    public DateTime? UploadDate { get; set; } = null;

    // TODO: User that uploaded

    public int FileId { get; set; }

    [ForeignKey( nameof( FileId ) )]
    public File? File { get; set; } = null;
}
