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

using Jaller.Core.Exceptions;
using Jaller.Standard.Configuration;
using LiteDB;

namespace Jaller.Core.Database;

internal abstract class BaseJallerDatabase : IDisposable
{
    // ---------------- Fields ----------------

    protected readonly LiteDatabase dbConnection;

    // ---------------- Constructor ----------------

    public BaseJallerDatabase( IJallerDatabaseConfig config )
    {
        BsonMapper.Global.EnumAsInteger = true;

        var connectionString = new ConnectionString
        {
            AutoRebuild = config.AutoRebuild,
            Connection = config.DirectConnection ? ConnectionType.Direct : ConnectionType.Shared,
            Filename = config.DatabaseLocation?.FullName ?? ":memory:",
            Password = config.EncryptionPassword,
            Upgrade = config.AutoUpgradeDb
        };

        this.dbConnection = new LiteDatabase( connectionString );
    }

    // ---------------- Methods ----------------

    public void BeginTransaction()
    {
        bool success = this.dbConnection.BeginTrans();
        if( success == false )
        {
            throw new DatabaseException( "Failed to begin transaction" );
        }
    }

    public void Commit()
    {
        bool success = this.dbConnection.Commit();
        if( success == false )
        {
            throw new DatabaseException( "Failed to commit transaction" );
        }
    }

    public void Rollback()
    {
        bool success = this.dbConnection.Rollback();
        if( success == false )
        {
            throw new DatabaseException( "Failed to rollback transaction" );
        }
    }

    public void Dispose()
    {
        this.dbConnection.Dispose();
    }
}
