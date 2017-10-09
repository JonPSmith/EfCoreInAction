// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.QueryObjects;
using DataNoSql;
using Raven.Client.Document;
using ServiceLayer.BookServices.RavenDb;

namespace ServiceLayer.BookServices.Concrete
{
    public class ListBooksNoSqlService
    {
        private readonly DocumentStore _store;

        public ListBooksNoSqlService(IRavenStore storeFactory)
        {
            _store = storeFactory.Store;
        }

        public IQueryable<BookListNoSql> SortFilterPage
            (NoSqlSortFilterPageOptions options)
        {
            if(_store == null)
                throw new InvalidOperationException("The RavenDb store was null. This can happen if the RavenDb connection string is null or empty.");

            using (var session = _store.OpenSession())
            {
                var booksQuery = session.Query<BookListNoSql>()
                    .OrderBooksBy(options.OrderByOptions)
                    .FilterBooksBy(options.FilterBy,     
                        options.FilterValue); 

                options.SetupRestOfDto(booksQuery);      

                return booksQuery.Page(options.PageNum-1,
                    options.PageSize);
            }

        }
    }
    /*********************************************************
    #A This starts by selecting the Books property in the Application's DbContext 
    #B Because this is a read-only query I add .AsNoTracking(). It makes the query faster
    #C It then uses the Select query object which will pick out/calculate the data it needs
    #D It then adds the commands to order the data using the given options
    #E Then it adds the commands to filter the data
    #F This stage sets up the number of pages and also makes sure PageNum is in the right range
    #G Finally it applies the paging commands
        * *****************************************************/
}