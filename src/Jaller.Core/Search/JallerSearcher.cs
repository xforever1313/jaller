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

using Jaller.Core.Database;
using Jaller.Standard;
using Jaller.Standard.Search;
using SethCS.Collections;
using SethCS.Extensions;

namespace Jaller.Core.Search;

internal sealed class JallerSearcher : IJallerSearch
{
    // ---------------- Fields ----------------

    private readonly IJallerCore core;

    private readonly JallerSearchCache searchCache;

    private readonly JallerDatabase fileData;

    // ---------------- Constructor ----------------

    public JallerSearcher( IJallerCore core, JallerSearchCache searchCache, JallerDatabase fileData )
    {
        this.core = core;
        this.searchCache = searchCache;
        this.fileData = fileData;
    }

    // ---------------- Properties ----------------

    public bool IsIndexing { get; private set; }

    public uint MaximumSearchQueryLength => 255;

    // ---------------- Methods ----------------

    public int GetKeywordCount()
    {
        return this.searchCache.SearchKeywords.Count();
    }

    public void Index( CancellationToken token )
    {
        try
        {
            this.core.Log.Information( "Indexing Started." );
            this.IsIndexing = true;
            
            // First, remove any files that may have been deleted.
            foreach( SearchKeyword keyword in this.searchCache.SearchKeywords.FindAll() )
            {
                // This keyword can be removed.
                if( ( keyword.Cids is null ) || ( keyword.Cids.Any() == false ) )
                {
                    this.searchCache.SearchKeywords.Delete( keyword.Keyword );
                }
                else
                {
                    keyword.Cids.RemoveWhere(
                        cid => this.fileData.Files.FindById( cid ) is null
                    );

                    if( keyword.Cids is null )
                    {
                        this.searchCache.SearchKeywords.Delete( keyword.Keyword );
                    }
                    else
                    {
                        this.searchCache.SearchKeywords.Update( keyword );
                    }
                }
            }

            // Next, populate the search keyword.
            foreach( IpfsFile file in this.fileData.Files.FindAll() )
            {
                var keywords = new List<string>();

                if( file.Tags?.Any() ?? false )
                {
                    foreach( string tag in file.Tags )
                    {
                        keywords.AddRange( tag.Split( ' ', StringSplitOptions.RemoveEmptyEntries ) );
                    }
                }

                // Include file name in the keywords
                string fileNameLower = file.FileName.ToLower();

                keywords.Add( fileNameLower );
                keywords.Add( Path.GetFileNameWithoutExtension( fileNameLower ) );

                foreach( string keyword in keywords )
                {
                    bool existing;
                    SearchKeyword? dbKeyword = this.searchCache.SearchKeywords.FindById( keyword );
                    if( dbKeyword is null )
                    {
                        dbKeyword = new SearchKeyword
                        {
                            Keyword = keyword,
                            Cids = new SequentialOrderIgnoredHashSet<string>()
                        };
                        existing = false;
                    }
                    else
                    {
                        existing = true;
                    }
                    
                    if( dbKeyword.Cids is null )
                    {
                        dbKeyword = dbKeyword with
                        {
                            Cids = new SequentialOrderIgnoredHashSet<string>()
                        };
                    }

                    dbKeyword.Cids.Add( file.Cid );

                    if( existing )
                    {
                        this.searchCache.SearchKeywords.Update( dbKeyword );
                    }
                    else
                    {
                        this.searchCache.SearchKeywords.Insert( dbKeyword );
                    }
                }
            }
        }
        finally
        {
            this.IsIndexing = false;
            this.core.Log.Information( "Indexing Finished." );
        }
    }

    public IReadOnlyCollection<JallerSearchResult> Search( string query )
    {
        if( query.Length > this.MaximumSearchQueryLength )
        {
            throw new ArgumentException(
                $"Query is too big, can be no more than 255 characters.  Got: {this.MaximumSearchQueryLength}."
            );
        }

        IEnumerable<string> keywords = query.NormalizeWhiteSpace().Split( ' ' ).Select( x => x.ToLower() );

        var foundCids = new HashSet<string>();
        foreach( string keyword in keywords )
        {
            SearchKeyword? searchResult = this.searchCache.SearchKeywords.FindById( keyword );
            if( ( searchResult is not null ) && ( searchResult.Cids is not null ) )
            {
                foreach( string cid in searchResult.Cids )
                {
                    foundCids.Add( cid );
                }
            }
        }

        return foundCids.Select( cid => this.fileData.Files.FindById( cid ) )
                .Where( file => file is not null )
                .Select( file => new JallerSearchResult( file.Cid, file.FileName ) )
                .ToArray();
    }
}
