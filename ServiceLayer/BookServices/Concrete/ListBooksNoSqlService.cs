// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.QueryObjects;
using DataNoSql;
using Raven.Client.Document;
using Raven.Client.Linq;
using ServiceLayer.BookServices.RavenDb;

namespace ServiceLayer.BookServices.Concrete
{
    public class ListBooksNoSqlService
    {
        private readonly IRavenQueryable<BookListNoSql> _query;

        public ListBooksNoSqlService(IRavenQueryable<BookListNoSql> query)
        {
            _query = query;
        }

        public IQueryable<BookListNoSql> SortFilterPage
            (NoSqlSortFilterPageOptions options)
        {
            if (_query == null)
                throw new InvalidOperationException(
                    "The RavenDb store was null. This can happen if the RavenDb connection string is null or empty.");

            var booksQuery = _query
                .OrderBooksBy(options.OrderByOptions)
                .FilterBooksBy(options.FilterBy,
                    options.FilterValue);

            options.SetupRestOfDto(booksQuery.Count());

            return booksQuery.Page(options.PageNum - 1,
                options.PageSize);
        }
    }
}