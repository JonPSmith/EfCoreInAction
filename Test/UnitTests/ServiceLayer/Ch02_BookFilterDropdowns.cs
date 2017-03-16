// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_BookFilterDropdowns
    {
        [Fact]
        public void DropdownByDate()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            const int numBooks = 5;
            using (var db = inMemDb.GetContextWithSetup())
            {
                db.Books.AddRange(EfTestData.CreateDummyBooks(numBooks, true));
                db.SaveChanges();
                var service = new BookFilterDropdownService(db);

                //ATTEMPT
                var dropDown = service.GetFilterDropDownValues(BooksFilterBy.ByPublicationYear);

                //VERIFY
                dropDown.Select(x => x.Value).ToArray().ShouldEqual(new []{"2014", "2013", "2012", "2011", "2010"});
            }
        }
    }
}