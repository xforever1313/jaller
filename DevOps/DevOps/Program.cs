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

using System.Reflection;
using Cake.Frosting;
using Seth.CakeLib;

namespace DevOps
{
    internal class Program
    {
        static int Main( string[] args )
        {
            string exeDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) ?? string.Empty;
            string repoRoot = Path.Combine(
                exeDir, // app
                "..", // Debug
                "..", // Bin
                "..", // DevOps
                "..", // Src
                ".."  // Root
            );

            return new CakeHost()
                .UseContext<BuildContext>()
                .SetToolPath( ".cake" )
                .InstallTool( new Uri( "nuget:?package=OpenCover&version=4.7.1221" ) )
                .InstallTool( new Uri( "nuget:?package=ReportGenerator&version=5.3.11" ) )
                .UseWorkingDirectory( repoRoot )
                .AddAssembly( SethCakeLib.GetAssembly() )
                .Run( args );
        }
    }
}