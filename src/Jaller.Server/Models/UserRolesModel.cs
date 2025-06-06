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

using System.Text.Json.Serialization;

namespace Jaller.Server;

public record class UserRolesModel
{
    // ---------------- Properties ----------------

    [JsonPropertyName( "logged_in" )]
    public bool IsLoggedIn { get; init; } = false;

    [JsonPropertyName( "is_approved_user" )]
    public bool IsApprovedUser { get; init; } = false;

    [JsonPropertyName( "is_editor" )]
    public bool IsEditor { get; init; } = false;

    [JsonPropertyName( "is_uploader" )]
    public bool IsUploader { get; init; } = false;

    [JsonPropertyName( "is_admin" )]
    public bool IsAdmin { get; init; } = false;
}