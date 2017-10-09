// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfCode;
using DataLayer.NoSql;
using DataNoSql;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_BookNoSqlDto
    {
        private readonly ITestOutputHelper _output;


        public Ch14_BookNoSqlDto(ITestOutputHelper output)
        {
            _output = output;

        }

        [Fact]
        public void TestSelectBookAuthors()
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
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var dto = context.Books.ProjectBook(1);

                //VERIFY
                dto.AuthorsOrdered.ShouldEqual("Author0000, CommonAuthor");
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        
    }
}