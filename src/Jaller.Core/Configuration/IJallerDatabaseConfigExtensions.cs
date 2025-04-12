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

using System.Text;
using Jaller.Core.Database;
using Jaller.Standard.Configuration;
using LiteDB;

namespace Jaller.Core.Configuration;

public static class IJallerDatabaseConfigExtensions
{
    public static string ToConnectionString( this IJallerDatabaseConfig dbConfig )
    {
        var builder = new StringBuilder();

        builder.Append( $"auto-rebuild={dbConfig.AutoRebuild};" );
        builder.Append( $"upgrade={dbConfig.AutoUpgradeDb};" );
        builder.Append( $"filename='{(dbConfig.DatabaseLocation?.FullName ?? BaseJallerDatabase.InMemoryDatabaseName).Replace( "'", @"\'" )}';" );
        builder.Append( $"connection={( dbConfig.DirectConnection ? ConnectionType.Direct : ConnectionType.Shared )};" );

        if( dbConfig.EncryptionPassword is not null )
        {
            builder.Append( $"password='{dbConfig.EncryptionPassword.Replace( "'", @"\'" ) }';" );
        }

        return builder.ToString();
    }
}
