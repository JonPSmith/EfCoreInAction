// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_ModelQueryFilter
    {
        private readonly ITestOutputHelper _output;

        public Ch03_ModelQueryFilter(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SoftDeleteOneBook()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void SoftDeleteOneBookIgnoreQueryFilters()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();

                //VERIFY
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(4);
            }
        }

        [Fact]
        public void SoftDeleteOneBookFind()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();
                var firstBook = context.Books.First();
                firstBook.SoftDeleted = true;
                context.SaveChanges();

                //ATTEMPT
                var softBook = context.Find<Book>(firstBook.BookId);

                //VERIFY
                softBook.ShouldNotBeNull();
            }
        }

        [Fact]
        public void SoftDeleteOneBookSetT()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();

                //VERIFY
                context.Set<Book>().Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void SoftDeleteOneBookNotSaved()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.Books.First().SoftDeleted = true;

                //VERIFY
                context.Books.Count().ShouldEqual(4);
            }
        }

    }
}