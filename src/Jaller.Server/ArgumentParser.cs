﻿//
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
using Mono.Options;
using SethCS.IO;

namespace Jaller.Server;

internal class ArgumentParser
{
    // ---------------- Fields ----------------

    private readonly OptionSet options;

    // ---------------- Constructor ----------------

    public ArgumentParser( string[] args )
    {
        this.options = new OptionSet
            {
                {
                    "h|help",
                    "Shows thie mesage and exits.",
                    v => this.ShowHelp = ( v is not null )
                },
                {
                    "version",
                    "Shows the version and exits.",
                    v => this.ShowVersion = ( v is not null )
                },
                {
                    "print_license",
                    "Prints the software license and exits.",
                    v => this.ShowLicense = ( v is not null )
                },
                {
                    "print_readme",
                    "Prints the readme file and exits.",
                    v => this.ShowReadme = ( v is not null )
                },
                {
                    "print_credits",
                    "Prints the third-party notices and credits and then exits.",
                    v => this.ShowCredits = ( v is not null )
                },
                {
                    "config_file=",
                    "The config file that contains the bot settings.",
                    v => ConfigFilePath = new FileInfo( v )
                },
                {
                    "check_config",
                    "Checks the configuration and ensures it is valid.  Exits with 0 if valid.",
                    v => this.CheckConfigOnly = ( v is not null )
                },
                {
                    "use_enter_to_exit",
                    "Allows the user to send enter to exit instead of CTRL+C.  Should only ever be used in a testing context.",
                    v => this.UseEnterToExit = ( v is not null )
                }
            };

        options.Parse( args );
    }

    // ---------------- Properties ----------------

    public bool ShowHelp { get; private set; }

    public bool ShowVersion { get; private set; }

    public bool ShowLicense { get; private set; }

    public bool ShowReadme { get; private set; }

    public bool ShowCredits { get; private set; }

    public bool CheckConfigOnly { get; private set; }

    public bool UseEnterToExit { get; private set; }

    public FileInfo? ConfigFilePath { get; private set; }

    // ---------------- Methods ----------------

    public void PrintHelp( TextWriter writer )
    {
        this.options.WriteOptionDescriptions( writer );
    }

    /// <summary>
    /// Returns the user-specified configuration path,
    /// or null to use the default path.
    /// </summary>
    public FileInfo? GetConfigFilePath()
    {
        if( this.ConfigFilePath is not null )
        {
            return this.ConfigFilePath;
        }

        string? envInfo = Environment.GetEnvironmentVariable( "JALLER_CONFIG_FILE" );
        if( string.IsNullOrEmpty( envInfo ) == false )
        {
            return new FileInfo( envInfo );
        }

        return null;
    }

    /// <summary>
    /// Performs any executions based on the passed in arguments.
    /// </summary>
    /// <returns>
    /// An exit code if the program should exit, otherwise null
    /// if the program should continue.
    /// </returns>
    public int? Execute()
    {
        if( this.ShowHelp )
        {
            Console.WriteLine( nameof( Jaller ) + " - A more advanced IPFS Gateway." );
            this.PrintHelp( Console.Out );
            return 0;
        }
        else if( this.ShowVersion )
        {
            PrintVersion();
            return 0;
        }
        else if( this.ShowLicense )
        {
            PrintLicense();
            return 0;
        }
        else if( this.ShowReadme )
        {
            PrintReadme();
            return 0;
        }
        else if( this.ShowCredits )
        {
            PrintCredits();
            return 0;
        }
        else
        {
            // Null means keep executing, do not exit early.
            return null;
        }
    }

    private static void PrintCredits()
    {
        string str = Resources.GetCredits();
        Console.WriteLine( str );
    }

    private static void PrintLicense()
    {
        string str = Resources.GetLicense();
        Console.WriteLine( str );
    }

    private static void PrintReadme()
    {
        string str = Resources.GetReadme();
        Console.WriteLine( str );
    }

    private static void PrintVersion()
    {
        Version? version = typeof( Program ).Assembly.GetName()?.Version;
        Console.WriteLine( $"Version: {version?.ToString( 3 ) ?? string.Empty}." );
    }
}
