// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_EfCoreContext
    {
        private readonly ITestOutputHelper _output;

        public Ch02_EfCoreContext(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateTestDataOk()
        {
            //SETUP

            //ATTEMPT
            var books = EfTestData.CreateFourBooks();

            //VERIFY
            books.Count.ShouldEqual(4);
            books.ForEach(x => x.AuthorsLink.Count.ShouldEqual(1));
            books[3].Reviews.Count.ShouldEqual(2);
            books[3].HasPromotion.ShouldBeTrue();
        }

        [Fact]
        public void TestWriteTestDataSqliteOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var books = EfTestData.CreateFourBooks();
                context.Books.AddRange(books);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(4);
                context.Books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }

        /// <summary>
        /// Thsi was written to see if the let statement in standard LINQ has a positive affect on the SQL command
        /// The answer is - it doens't, i.e. the SQL produced has two SELEC COUNT(*)... statements, not one
        /// </summary>
        [Fact]
        public void TestStandardLinqLetOk()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var books = (from book in context.Books
                            let count = book.Reviews.Count
                            select new { Count1 = count, Count2 = count}
                    ).ToList();

                //VERIFY
                books.First().Count1.ShouldEqual(books.First().Count2);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}