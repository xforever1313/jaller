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

namespace Jaller.Standard.Configuration;

public interface IJallerDatabaseConfig
{
    /// <summary>
    /// The location of where to put the LiteDB database.
    /// 
    /// This is defaulted to the user's application data folder
    /// inside of a "Jaller" folder.
    /// 
    /// Set to null to use an "in memory" database (NOT RECOMMENDED EXCEPT IN A TEST ENVIRONMENT).
    /// </summary>
    FileInfo? DatabaseLocation { get; }

    /// <summary>
    /// How to open the database.
    /// 
    /// Set to true for a direct connection, where the engine will
    /// open the datafile in exclusive mode and will keep 
    /// it open until Dispose().
    /// The datafile cannot be opened by another process.
    /// This is the recommended mode because it’s faster and cachable.
    /// 
    /// Set to false to use a shared connection.  The engine will be close 
    /// the datafile after each operation. 
    /// Locks are made using Mutex.
    /// This is more expensive but you can open same file from multiple processes.
    /// </summary>
    /// <remarks>
    /// From here: https://colinchsql.github.io/2023-10-13/10-17-25-480023-sqlite-database-connection-pooling-strategies/
    /// Connection pooling is a technique that involves creating 
    /// and maintaining a pool of database connections,
    /// which are then reused by multiple threads or processes.
    /// Instead of establishing a new connection each time,
    /// a thread can request a connection from the pool and 
    /// return it when it is finished.
    /// </remarks>
    bool DirectConnection { get; }

    /// <summary>
    /// If last close database exception result a invalid data state,
    /// rebuild datafile on next open.
    /// 
    /// Defaulted to false.
    /// </summary>
    bool AutoRebuild { get; }

    /// <summary>
    /// Check if datafile is of an older version and upgrade it before opening.
    /// Defaulted to false.
    /// This should really only be set to true if instructed to in the release notes
    /// when upgrading releases.
    /// </summary>
    bool AutoUpgradeDb { get; }

    /// <summary>
    /// Set to a non-null value to AES encrypt the datafile with this password.
    /// Set to null to use no encryption (default).
    /// </summary>
    string? EncryptionPassword { get; }
}
