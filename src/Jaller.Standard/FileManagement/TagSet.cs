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

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Jaller.Standard.FileManagement
{
    public sealed class TagSet : HashSet<string>, IEquatable<TagSet>
    {
        // ---------------- Constructor ----------------

        public TagSet() :
            base( new TagSetEqualityComparer() )
        {
        }

        // ---------------- Methods ----------------

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append( "[" );
            foreach( var tag in this )
            {
                builder.Append( $"{tag}," );
            }

            builder.Remove( builder.Length - 1, 1 );
            builder.Append( "]" );

            return builder.ToString();
        }

        public override bool Equals( object? obj )
        {
            return Equals( obj as TagSet );
        }

        public bool Equals( TagSet? other )
        {
            if( other is null )
            {
                return false;
            }

            return this.SequenceEqual( other );
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach( string tag in this )
            {
                hashCode = HashCode.Combine( tag );
            }

            return hashCode;
        }

        // ---------------- Helper Classes ----------------

        private sealed class TagSetEqualityComparer : IEqualityComparer<string?>
        {
            public bool Equals( string? x, string? y )
            {
                return x == y;
            }

            public int GetHashCode( [DisallowNull] string? obj )
            {
                return obj.GetHashCode();
            }
        }
    }
}
