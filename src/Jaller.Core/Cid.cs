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

using PackgeIpfs = Ipfs;

namespace Jaller.Core
{
    /// <summary>
    /// The CID of a file that is hosted.
    /// </summary>
    public sealed record class Cid
    {
        // ---------------- Constructor ----------------

        private Cid( string version0, string version1 )
        {
            this.Version0Cid = version0;
            this.Version1Cid = version1;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The CID using the version 0 algorithm.
        /// 
        /// Version 0 used base 58-encoded multihashes.
        /// If the CID is 46 characters and starts with "Qm",
        /// its most likely version 0.
        /// </summary>
        public string Version0Cid { get; }

        /// <summary>
        /// The CID using the version 1 hashing algorithm.
        /// 
        /// This includes:
        /// - A multibase prefix
        /// - The CID version identifier
        /// - A mulitcoded identifier.
        /// </summary>
        public string Version1Cid { get; }

        // ---------------- Methods ----------------

        public static Cid? TryParse( string hash )
        {
            try
            {
                PackgeIpfs.Cid innerCid = PackgeIpfs.Cid.Decode( hash );

                string version0;
                string version1;
                if( innerCid.Version == 0 )
                {
                    version0 = hash;
                    innerCid.Version = 1;
                    version1 = innerCid.Encode();
                }
                else if( innerCid.Version == 1 )
                {
                    version1 = hash;
                    innerCid.Version = 0;
                    version0 = innerCid.Encode();
                }
                else
                {
                    return null;
                }

                return new Cid( version0, version1 );
            }
            catch( Exception )
            {
                return null;
            }
        }

        public static Cid Parse( string hash )
        {
            Cid? cid = TryParse( hash );
            if( cid is null )
            {
                throw new FormatException(
                    "Invalid CID: " + hash
                );
            }

            return cid;
        }
    }
}
