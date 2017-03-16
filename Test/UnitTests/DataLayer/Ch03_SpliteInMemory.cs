// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_SpliteInMemory
    {
        private readonly ITestOutputHelper _output;

        public Ch03_SpliteInMemory(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSqliteInMemoryBasicOk()
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

        //This Unit test should be fixed by release 1.2 - see https://github.com/aspnet/EntityFramework/pull/6980
        [Fact]
        public void TestSqliteInMemoryTwoContextsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var books = EfTestData.CreateFourBooks();
                context.Books.AddRange(books);
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options))
            {
                context.Database.OpenConnection();
                //VERIFY
                context.Books.Count().ShouldEqual(4);
                context.Books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }


    }
}