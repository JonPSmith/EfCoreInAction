// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.AdminServices.Concrete;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch13_ChangePriceOfferService
    {
        private readonly ITestOutputHelper _output;

        public Ch13_ChangePriceOfferService(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddPromotion()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var service = new ChangePriceOfferService(context);
                const int bookId = 1;

                //ATTEMPT
                var dto = service.GetOfferData(bookId);
                dto.NewPrice = 10;
                dto.PromotionalText = "Unit Test";
                var error = service.AddPromotion(dto);
                context.SaveChanges();

                //VERIFY
                error.ShouldBeNull();
                var book = context.Books.Single(x => x.BookId == bookId);
                book.HasPromotion.ShouldBeTrue();
                book.ActualPrice.ShouldEqual(10);
                book.OrgPrice.ShouldEqual(dto.OrgPrice);
            }
        }

        [Fact]
        public void TestRemovePromotion()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var service = new ChangePriceOfferService(context);
                const int bookId = 4;

                //ATTEMPT
                service.RemovePromotion(bookId);
                context.SaveChanges();

                //VERIFY
                var book = context.Books.Single(x => x.BookId == bookId);
                book.HasPromotion.ShouldBeFalse();
                book.ActualPrice.ShouldEqual(book.OrgPrice);
            }
        }



    }
}