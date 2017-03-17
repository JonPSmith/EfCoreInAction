// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using EfCoreInAction.Controllers;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch05_EfCoreContextAsync
    {
        private readonly ITestOutputHelper _output;

        public Ch05_EfCoreContextAsync(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestWriteTestDataSqliteOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var books = EfTestData.CreateFourBooks();
                context.Books.AddRange(books);
                await context.SaveChangesAsync();

                //VERIFY
                context.Books.Count().ShouldEqual(4);
                context.Books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestReadSqliteAsyncOk()
        {

            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                context.Books.AddRange(EfTestData.CreateFourBooks());
                await context.SaveChangesAsync();

                //ATTEMPT
                var books = await context.Books.ToListAsync();

                //VERIFY
                books.Count.ShouldEqual(4);
                books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestMapBookToDtoAsyncOk()
        {

            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                context.Books.AddRange(EfTestData.CreateFourBooks());
                await context.SaveChangesAsync();

                //ATTEMPT
                var result = await context.Books.Select(p => 
                    new BookListDto
                    {
                        ActualPrice = p.Promotion == null
                            ? p.Price
                            : p.Promotion.NewPrice,
                        ReviewsCount = p.Reviews.Count,
                    }).ToListAsync();

                //VERIFY
                result.Count.ShouldEqual(4);
            }
        }
    }
}