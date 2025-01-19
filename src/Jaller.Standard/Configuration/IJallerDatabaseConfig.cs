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
    /// The location of where to put the SQLite database.
    /// 
    /// This is defaulted to the user's application data folder
    /// inside of a "Jaller" folder.
    /// </summary>
    FileInfo SqliteDatabaseLocation { get; }

    /// <summary>
    /// Whether or not to enabling pooling for sqlite.  Defaulted
    /// to true.
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
    bool SqlitePool { get; }
}
