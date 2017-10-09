// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataNoSql;
using ServiceLayer.BookServices.RavenDb;

namespace ServiceLayer.BookServices
{
    public class BookListNoSqlCombinedDto
    {
        public BookListNoSqlCombinedDto(NoSqlSortFilterPageOptions sortFilterPageData, IEnumerable<BookListNoSql> booksList)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public NoSqlSortFilterPageOptions SortFilterPageData { get; private set; }

        public IEnumerable<BookListNoSql> BooksList { get; private set; }
    }
}