// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using DataLayer.NoSql;
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
        public void TestSelectBookAuthors()
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

        
    }
}