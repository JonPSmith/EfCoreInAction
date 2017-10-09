// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace DataNoSql
{
    public class BookById : AbstractIndexCreationTask<BookListNoSql>
    {
        public BookById()
        {
            Map = books => from book in books
                select new { book.Id };
            Indexes.Add(x => x.Id, FieldIndexing.Default);
        }
    }

    public class BookByActualPrice : AbstractIndexCreationTask<BookListNoSql>
    {
        public BookByActualPrice()
        {
            Map = books => from book in books
                select new { book.ActualPrice };
            Indexes.Add(x => x.Id, FieldIndexing.Default);
        }
    }

    public class BookByVotes : AbstractIndexCreationTask<BookListNoSql>
    {
        public BookByVotes()
        {
            Map = books => from book in books
                select new { book.ReviewsAverageVotes };
            Indexes.Add(x => x.Id, FieldIndexing.Default);
        }
    }
}