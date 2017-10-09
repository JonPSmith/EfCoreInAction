// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using DataLayer.NoSql;
using DataNoSql;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_CreateBookListNoSql
    {
        private readonly ITestOutputHelper _output;


        public Ch14_CreateBookListNoSql(ITestOutputHelper output)
        {
            _output = output;

        }

        [Fact]
        public void TestProjectBook()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var dto = context.Books.ProjectBook(1);

                //VERIFY
                dto.AuthorsOrdered.ShouldEqual("Author0000, CommonAuthor");
            }
        }

        [Fact]
        public void TestProjectBooks()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var results = context.Books.ProjectBooks().ToList();

                //VERIFY
                results.Count.ShouldEqual(10);
                results.ForEach(x => x.AuthorsOrdered.ShouldEqual($"Author{(x.GetIdAsInt()-1):D4}, CommonAuthor"));
            }
        }


    }
}