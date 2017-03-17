// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch05_AsyncAwait
    {
        [Fact]
        public async Task RunBookListAsync()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();
                var service = new ListBooksService(context);

                //ATTEMPT
                var numBooks = await service.SortFilterPage(new SortFilterPageOptions()).ToListAsync();

                //VERIFY
                numBooks.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public async Task GetNumBooksAsync()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var numBooks = await GetNumBooksAsync(context);

                //VERIFY
                numBooks.ShouldEqual(4);
            }
        }

        public async Task<int> //#A
            GetNumBooksAsync(EfCoreContext context) //#B
        {
            return await //#C
                context.Books
                .CountAsync(); //#D
        }
        /**********************************************
        #A The 'async Task<T>' is the way we define a method as having an await in it
        #B Async methods names end, by convention, with Async
        #C The await keyword tells the compiler to place a task call, with a callback to continue after it has finished
        #D The CountAsync is an EF Core method that returns the count of the query, in this case the count of all the books in the database
         * *********************************************/

        private Task<int> GetNumBooksTask(EfCoreContext context)
        {
            return context.Books.CountAsync();
        }
    }
}