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

using SethCS.Extensions;

namespace Jaller.Server.Models;

public record class ImportModel(
    IFormFile? File,
    bool OverwriteExistingFiles = false
)
{
    /// <summary>
    /// Validates the model.  If there are any error messages, this returns
    /// the error message.
    /// 
    /// Returns null on no error.
    /// </summary>
    public string? TryValidate()
    {
        if( this.File is null )
        {
            return $"{nameof( this.File )} was null!";
        }
        else if( ".xml".EqualsIgnoreCase( Path.GetExtension( this.File.FileName ) ) == false )
        {
            return "File name's extension does not end in .xml";
        }
        else if( 
            ( "application/xml".EqualsIgnoreCase( this.File.ContentType ) == false ) &&
            ( "text/xml".EqualsIgnoreCase( this.File.ContentType ) == false )
        )
        {
            return "Invalid Content Type";
        }

        return null;
    }
}
