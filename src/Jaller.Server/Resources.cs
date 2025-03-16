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

using Jaller.Contracts;
using Jaller.Contracts.About;
using Jaller.Core;
using Jaller.Markdown;
using Jaller.Standard.Configuration;
using SethCS.IO;

namespace Jaller.Server;

public class Resources
{
    // ---------------- Constructor ----------------

    static Resources()
    {
        ContractsVersion = typeof( JallerContracts ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unknown";
        CoreVersion = typeof( JallerCore ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unknown";
        ServerVersion = typeof( Resources ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unknown";
        StandardVersion = typeof( IJallerConfig ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unknown";

        LicenseAsHtml = MarkdownToHtmlConverter.Convert( GetLicense() );
        CreditsAsHtml = MarkdownToHtmlConverter.Convert( GetCredits() );
        ReadmeAsHtml = MarkdownToHtmlConverter.Convert( GetReadme() );
    }

    // ---------------- Properties ----------------

    public static string ContractsVersion { get; }

    public static string CoreVersion { get; }

    public static string ServerVersion { get; }

    public static string StandardVersion { get; }

    public static string LicenseAsHtml { get; }

    public static string CreditsAsHtml { get; }

    public static string ReadmeAsHtml { get; }

    // ---------------- Methods ----------------

    public static JallerVersionInfo GetVersionInfo()
    {
        return new JallerVersionInfo( ContractsVersion, CoreVersion, ServerVersion, StandardVersion );
    }

    public static string GetLicense()
    {
        string str = AssemblyResourceReader.ReadStringResource(
            typeof( Resources ).Assembly, $"{nameof( Jaller )}.{nameof( Server )}.License.md"
        );

        return str;
    }

    public static string GetReadme()
    {
        string str = AssemblyResourceReader.ReadStringResource(
            typeof( Program ).Assembly, $"{nameof( Jaller )}.{nameof( Server )}.Readme.md"
        );

        return str;
    }

    public static string GetCredits()
    {
        string str = AssemblyResourceReader.ReadStringResource(
            typeof( Program ).Assembly, $"{nameof( Jaller )}.{nameof( Server )}.Credits.md"
        );

        return str;
    }

    public static string GetDefaultConfiguration()
    {
        string str = AssemblyResourceReader.ReadStringResource(
              typeof( Program ).Assembly, $"{nameof( Jaller )}.{nameof( Server )}.DefaultJallerConfig.cs"
        );

        return str;
    }
}