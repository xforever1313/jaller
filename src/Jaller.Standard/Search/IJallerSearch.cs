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

namespace Jaller.Standard.Search
{
    public interface IJallerSearch
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// Returns true if the search engine is in the middle of indexing,
        /// which may result in inaccurrate results.
        /// </summary>
        bool IsIndexing { get; }

        /// <summary>
        /// The maximum allowed search query.
        /// </summary>
        uint MaximumSearchQueryLength { get; }

        // ---------------- Methods ----------------

        /// <summary>
        /// Indexes the search tables.
        /// </summary>
        void Index( CancellationToken token );

        /// <summary>
        /// Performs a search with the given query, and returns the result.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IReadOnlyCollection<JallerSearchResult> Search( string query );
    }
}
