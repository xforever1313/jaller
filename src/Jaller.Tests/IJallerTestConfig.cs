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

namespace Jaller.Tests
{
    public interface IJallerTestConfig
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// The name of the test fixture. This should be unique across all tests.
        /// </summary>
        string TestFixtureName { get; }

        /// <summary>
        /// The work directory for the tests.  Where it will dump files.
        /// </summary>
        DirectoryInfo TestDirectory { get; }

        /// <summary>
        /// Path to the Jaller Config file.
        /// </summary>
        FileInfo ConfigFile { get; }

        /// <summary>
        /// The port number to listen on.
        /// </summary>
        ushort PortNumber { get; }

        // ---------------- Methods ----------------

        /// <summary>
        /// Converts this config to a Jaller Configuration file that
        /// Jaller can then consume.
        /// </summary>
        string ToJallerConfigFile();
    }
}
