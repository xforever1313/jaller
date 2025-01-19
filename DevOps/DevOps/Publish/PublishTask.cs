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

using System.Text;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Publish
{
    [TaskName( "publish" )]
    [IsDependentOn( typeof( PublishWinX64 ) )]
    [IsDependentOn( typeof( PublishWinArm64 ) )]
    [IsDependentOn( typeof( PublishLinuxX64 ) )]
    [IsDependentOn( typeof( PublishLinuxArm64 ) )]
    public sealed class PublishTask : DevopsTask
    {
        // ---------------- Methods ----------------

        public override void Run( BuildContext context )
        {
            // Output the Jaller version.  It will come in handy for deploying.
            string version = context.GetJallerVersion().ToString( 3 );
            FilePath versionFile = context.DistFolder.CombineWithFilePath( new FilePath( "version.txt" ) );
            File.WriteAllText( versionFile.FullPath, version );

            File.WriteAllText(
                context.DistFolder.CombineWithFilePath( new FilePath( "deploy.sh" ) ).FullPath,
                GenerateDeployScript( context, version )
            );
        }

        private string GenerateDeployScript( BuildContext context, string JallerVersion )
        {
            IEnumerable<FilePath> nugetFiles = context.Globber.GetFiles(
                new GlobPattern( context.DistFolder.CombineWithFilePath( "nuget/*.* " ).FullPath )
            );

            IEnumerable<FilePath> zipFiles = context.Globber.GetFiles(
                new GlobPattern( context.DistFolder.CombineWithFilePath( "zip/*.zip " ).FullPath )
            );

            string GetEnvVariable( string variableName )
            {
                return $"${variableName}";
            }

            var builder = new StringBuilder();

            void AddCommand( string command )
            {
                builder.AppendLine( $"echo '{command}'" );
                builder.AppendLine( command + " || exit 1" );
            }

            builder.AppendLine( "#!/bin/bash" );

            string releasePath = $"/home/{GetEnvVariable( "SSHUSER" )}/files.shendrick.net/projects/jaller/releases";
            string sshOptions = $"-v -o BatchMode=yes -o StrictHostKeyChecking=accept-new -i \"{GetEnvVariable( "WEBSITE_KEY" )}\"";
            string sshLogin = $"ssh {sshOptions} {GetEnvVariable( "SSHUSER" )}@files.shendrick.net";

            AddCommand( sshLogin + $" mkdir -p {releasePath}/{JallerVersion}" );

            AddCommand( $"scp {sshOptions} \"{GetEnvVariable( "WORKSPACE" )}/checkout/dist/version.txt\" {GetEnvVariable( "SSHUSER" )}@files.shendrick.net:{releasePath}/{JallerVersion}/version.txt" );

            foreach( FilePath nugetFile in nugetFiles )
            {
                AddCommand( $"scp {sshOptions} \"{GetEnvVariable( "WORKSPACE" )}/checkout/dist/nuget/{nugetFile.GetFilename()}\" {GetEnvVariable( "SSHUSER" )}@files.shendrick.net:{releasePath}/{JallerVersion}/{nugetFile.GetFilename()}" );
            }

            foreach( FilePath zipFile in zipFiles )
            {
                AddCommand( $"scp {sshOptions} \"{GetEnvVariable( "WORKSPACE" )}/checkout/dist/zip/{zipFile.GetFilename()}\" {GetEnvVariable( "SSHUSER" )}@files.shendrick.net:{releasePath}/{JallerVersion}/{zipFile.GetFilename()}" );
            }

            AddCommand( sshLogin + $" \"touch {releasePath}/latest && rm {releasePath}/latest && ln -s {releasePath}/{JallerVersion} {releasePath}/latest\"" );

            builder.AppendLine( "exit 0" );

            return builder.ToString();
        }
    }

    [TaskName( "publish_win_x64" )]
    public sealed class PublishWinX64 : BasePublishTask
    {
        // ---------------- Properties ----------------

        public override string Rid => "win-x64";
    }

    [TaskName( "publish_win_arm64" )]
    public sealed class PublishWinArm64 : BasePublishTask
    {
        // ---------------- Properties ----------------

        public override string Rid => "win-arm64";
    }

    [TaskName( "publish_linux_x64" )]
    public sealed class PublishLinuxX64 : BasePublishTask
    {
        // ---------------- Properties ----------------

        public override string Rid => "linux-x64";
    }

    [TaskName( "publish_linux_arm64" )]
    public sealed class PublishLinuxArm64 : BasePublishTask
    {
        // ---------------- Properties ----------------

        public override string Rid => "linux-arm64";
    }

    public abstract class BasePublishTask : DevopsTask
    {
        // ---------------- Properties ----------------

        public abstract string Rid { get; }

        // ---------------- Methods ----------------

        public override void Run( BuildContext context )
        {
            context.EnsureDirectoryExists( context.DistFolder );

            DirectoryPath looseFilesDir = context.LooseFilesDistFolder;
            context.EnsureDirectoryExists( looseFilesDir );

            DirectoryPath archFolder = looseFilesDir.Combine( new DirectoryPath( this.Rid ) );
            context.EnsureDirectoryExists( archFolder );
            context.CleanDirectory( archFolder );

            DirectoryPath binFolder = archFolder.Combine( new DirectoryPath( "bin" ) );
            context.EnsureDirectoryExists( binFolder );

            DirectoryPath pluginsFolder = archFolder.Combine( new DirectoryPath( "plugins" ) );
            context.EnsureDirectoryExists( pluginsFolder );

            var publishOptions = new DotNetPublishSettings
            {
                Configuration = "Release",
                OutputDirectory = binFolder.ToString(),
                MSBuildSettings = context.GetBuildSettings(),
                PublishReadyToRun = true,
                PublishReadyToRunShowWarnings = true,
                SelfContained = false,
                Runtime = this.Rid
            };

            context.Information( "Publishing Service..." );
            context.DotNetPublish( context.ServiceProject.ToString(), publishOptions );

            context.Information( string.Empty );

            context.Information( "Coping readme files..." );
            CopyRootFile( context, "Readme.md", archFolder );
            CopyRootFile( context, "Credits.md", archFolder );
            CopyRootFile( context, "License.md", archFolder );

            context.Information( "Zipping..." );
            context.EnsureDirectoryExists( context.ZipFilesDistFolder );
            FilePath zipFile = context.ZipFilesDistFolder.CombineWithFilePath( $"Jaller_{this.Rid}.zip" );
            if( context.FileExists( zipFile ) )
            {
                context.DeleteFile( zipFile );
            }
            context.Zip( archFolder, zipFile );

            context.Information( "Done Publishing!" );
        }

        private void CopyRootFile( BuildContext context, FilePath fileName, DirectoryPath archFolder )
        {
            fileName = context.RepoRoot.CombineWithFilePath( fileName );
            context.Information( $"Copying '{fileName}' to '{archFolder}'." );
            context.CopyFileToDirectory( fileName, archFolder );
        }
    }
}
