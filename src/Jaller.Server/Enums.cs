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

namespace Jaller.Server;

public enum Roles
{
    /// <summary>
    /// A normal user, can only login to view and download private files.
    /// </summary>
    User,

    /// <summary>
    /// A user that can modify file metadata, but can not upload.
    /// </summary>
    Editor,

    /// <summary>
    /// A user that can upload files to IPFS, and add metadata.
    /// </summary>
    Uploader,

    /// <summary>
    /// A user that can control everything.
    /// </summary>
    Admin
}

/// <summary>
/// If the file is renderable, how it should be rendered.
/// </summary>
public enum RenderableMimeType
{
    /// <summary>
    /// File is not renderable.
    /// </summary>
    NotRenderable,

    /// <summary>
    /// File is an image, and can be rendered via img src.
    /// </summary>
    Image,

    /// <summary>
    /// File is an audio file, and can be rendered via the audio tag.
    /// </summary>
    Audio,

    /// <summary>
    /// File is a video file, and can be rendered via the video tag.
    /// </summary>
    Video,

    /// <summary>
    /// File is a PDF, and can be rendered with the object tag.
    /// </summary>
    Pdf,

    /// <summary>
    /// File is plain-text.
    /// </summary>
    PlainText
}

public static class EnumExtensions
{
    public static string GetRoleName( this Roles role )
    {
        return role switch
        {
            Roles.User => "User",
            Roles.Editor => "Editor",
            Roles.Uploader => "Uploader",
            Roles.Admin => "Administrator",
            _ => throw new ArgumentException( $"Invalid role: {role}", nameof( role ) ),
        };
    }
}
