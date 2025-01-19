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

namespace Jaller.Standard.Configuration
{
    public interface IJallerIpfsGatewayConfig
    {
        /// <summary>
        /// The URL to the IPFS gateway server that runs RPC API, including the port.
        /// This is NOT the URL to the HTTP Gateway, (usually port 8080), but the RPC API
        /// (usually port 5001).
        /// </summary>
        Uri GatwayUrl { get; }

        /// <summary>
        /// How much to multiply the default timeout by
        /// when downloading something from the IPFS gateway in the event
        /// the network between the two is slow.
        /// 
        /// The timeout is calculated by ( <see cref="TimeoutMultiplier"/> * (fileSize / 100Mbps) ) + 100 seconds.
        /// 
        /// If this is set to 0, this makes an infinite timeout.
        /// </summary>
        uint TimeoutMultiplier { get; }
    }
}
