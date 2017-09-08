// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_ListSortFilterPageDto
    {
        [Fact]

        public void DefaultValues()
        {
            //SETUP

            //ATTEMPT
            var sfpDto = new SortFilterPageOptions();

            //VERIFY
            sfpDto.OrderByOptions.ShouldEqual(OrderByOptions.SimpleOrder);
            sfpDto.FilterBy.ShouldEqual(BooksFilterBy.NoFilter);
            sfpDto.FilterValue.ShouldBeNull();
            sfpDto.PageNum.ShouldEqual(1);
            sfpDto.PageSize.ShouldEqual(SortFilterPageOptions.DefaultPageSize);
            sfpDto.NumPages.ShouldEqual(0);
            sfpDto.PrevCheckState.ShouldBeNull();
        }


        [Theory]
        [InlineData(BooksFilterBy.ByVotes, "Dummy", 10, 2, 2)]
        [InlineData(BooksFilterBy.ByPublicationYear, "2010", 5, 1, 3)]
        public void SetupRestOfDto(BooksFilterBy filterBy, string filterValue, int pageSize, 
            int expectedPageNum, int expectedNumPages)
        {
            //SETUP
            var inMemDb = new SqliteInMemory();
            const int numBooks = 12;
            using (var db = inMemDb.GetContextWithSetup())
            {
                db.Books.AddRange(EfTestData.CreateDummyBooks(numBooks, false));
                db.SaveChanges();

                var sfpDto = new SortFilterPageOptions
                {
                    FilterBy = BooksFilterBy.ByVotes,
                    FilterValue = "Dummy",
                    PageNum = 2
                };

                //need to do this to to setup PrevCheckState 
                sfpDto.SetupRestOfDto(db.Books);

                //ATTEMPT
                sfpDto.PageNum = 2;
                sfpDto.FilterBy = filterBy;
                sfpDto.FilterValue = filterValue;
                sfpDto.PageSize = pageSize;
                sfpDto.SetupRestOfDto(db.Books);

                //VERIFY
                sfpDto.PageNum.ShouldEqual(expectedPageNum);
                sfpDto.NumPages.ShouldEqual(expectedNumPages);
            }
        }
    }
}