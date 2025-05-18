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

using System;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Jaller.Standard;
using Jaller.Standard.Ipfs;

namespace Jaller.Core.Ipfs
{
    internal sealed class JallerIpfsManager : IJallerIpfsManager
    {
        // ---------------- Fields ----------------

        private readonly IJallerCore core;
        private readonly HttpClient httpClient;

        private const string apiPath = "/api/v0";

        // ---------------- Constructor ----------------

        public JallerIpfsManager( IJallerCore core, HttpClient client )
        {
            this.core = core;
            this.httpClient = client;
        }

        // ---------------- Methods ----------------

        public Stream GetVersionInfo()
        {
            const string versionPath = apiPath + "/version?all=true";

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri( versionPath, UriKind.Relative ),
                Method = HttpMethod.Post
            };

            HttpResponseMessage response = this.httpClient.Send( request );
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStream();
        }

        public Stream GetFile( string cid )
        {
            string versionPath = apiPath + $"/cat?progress=false&arg={cid}";

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri( versionPath, UriKind.Relative ),
                Method = HttpMethod.Post
            };

            HttpResponseMessage response = this.httpClient.Send( request );
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStream();
        }

        public async Task<IpfsUploadResult> UploadFileAsync( string fileName, Stream stream, CancellationToken token )
        {
            const string uploadUrl = apiPath + "/add?quieter=true&progress=false&cid-version=1&pin=true";

            TimeSpan timeout;
            if( this.core.Config.Ipfs.TimeoutMultiplier <= 0 )
            {
                timeout = Timeout.InfiniteTimeSpan;
            }
            else
            {
                long fileSizeBits = stream.Length * 8;

                long seconds = fileSizeBits / ( 100 * 1000 * 1000 ); // Assuming 100 Mbps connection.

                // 100 seconds is the default timeout for HttpClient, add that
                // in case its not a smooth 100Mbps connection for wiggle room.
                seconds = ( seconds * this.core.Config.Ipfs.TimeoutMultiplier ) + 100;
                timeout = TimeSpan.FromSeconds( seconds );
            }

            using var fileVaue = new StreamContent( stream );
            var dataContent = new MultipartFormDataContent
            {
                { fileVaue, "path", Path.GetFileName( fileName ) }
            };

            using var tokenSource = new CancellationTokenSource( timeout );

            HttpResponseMessage response = await this.httpClient.PostAsync(
                uploadUrl,
                dataContent,
                tokenSource.Token
            );

            if( response.IsSuccessStatusCode == false )
            {
                string body = await response.Content.ReadAsStringAsync( token );

                throw new HttpRequestException(
                    $"HTTP Error {response.StatusCode} when uploading: {body}"
                );
            }
            else if( response.Content is null )
            {
                throw new HttpRequestException(
                    "HTTP Content was null!"
                );
            }

            InnerIpfsUploadResult? jsonResponse = response.Content.ReadFromJsonAsync<InnerIpfsUploadResult>().Result;
            if( jsonResponse is null )
            {
                throw new HttpRequestException(
                    "JSON Response was null!"
                );
            }
            else if( string.IsNullOrWhiteSpace( jsonResponse.Hash ) )
            {
                throw new HttpRequestException(
                    "Received JSON response did not containa hash!"
                );
            }

            Cid cid = Cid.Parse( jsonResponse.Hash );

            return new IpfsUploadResult( CidV1: cid.Version1Cid, FileName: fileName );
        }

        // ---------------- Helper Classes ----------------

        private sealed class InnerIpfsUploadResult
        {
            // ---------------- Properties ----------------

            [JsonPropertyName( "Name" )]
            public string? FileName { get; set; }

            [JsonPropertyName( "Hash" )]
            public string? Hash { get; set; }

            [JsonPropertyName( "Size" )]
            public long? FileSize { get; set; }
        }
    }
}
