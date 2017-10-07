// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Mocks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_INoSqlUpdater
    {
        private readonly ITestOutputHelper _output;


        public Ch14_INoSqlUpdater(ITestOutputHelper output)
        {
            _output = output;

        }

        [Fact]
        public void TestNoSqlUpdateNewBook()
        {
            //SETUP
            var fakeUpdater = new FakeNoSqlUpdater();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options, fakeUpdater))
            {
                context.Database.EnsureCreated();
                var book = EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Books.Add(book);
                context.SaveChanges();

                //VERIFY    
                fakeUpdater.AllLogs.ShouldEqual("Create: BookId = 1");
            }
        }

        [Fact]
        public void TestNoSqlUpdateUpdatedBook()
        {
            //SETUP
            var fakeUpdater = new FakeNoSqlUpdater();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options, fakeUpdater))
            {
                //ATTEMPT
                context.Books.First().Title = "new title";
                context.SaveChanges();

                //VERIFY    
                fakeUpdater.AllLogs.ShouldEqual("Update: BookId = 1");
            }
        }

        [Fact]
        public void TestNoSqlUpdateDeleteBook()
        {
            //SETUP
            var fakeUpdater = new FakeNoSqlUpdater();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options, fakeUpdater))
            {
                //ATTEMPT
                context.Remove(context.Books.First());
                context.SaveChanges();

                //VERIFY    
                fakeUpdater.AllLogs.ShouldEqual("Delete: BookId = 1");
            }
        }

        [Fact]
        public void TestNoSqlUpdateSoftDeleteBook()
        {
            //SETUP
            var fakeUpdater = new FakeNoSqlUpdater();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options, fakeUpdater))
            {
                //ATTEMPT
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();

                //VERIFY    
                fakeUpdater.AllLogs.ShouldEqual("Delete: BookId = 1");
            }
        }

        [Fact]
        public void TestNoSqlUpdateSoftDeleteUndoneBook()
        {
            //SETUP
            var fakeUpdater = new FakeNoSqlUpdater();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options, fakeUpdater))
            {
                //ATTEMPT
                context.Books.IgnoreQueryFilters().First().SoftDeleted = false;
                context.SaveChanges();

                //VERIFY    
                fakeUpdater.AllLogs.ShouldEqual("Create: BookId = 1");
            }
        }
    }
}