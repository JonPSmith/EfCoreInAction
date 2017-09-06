// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using DataLayer.EfCode;
using DataLayer.SqlCode;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch13_BookListDtoImproved
    {
        private readonly ITestOutputHelper _output;

        public Ch13_BookListDtoImproved(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookListDtoImproved()
        {
            //SETUP
            var options = this.ClassUniqueDatabaseSeeded4Books();
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\",
                UdfDefinitions.SqlScriptName);

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(filepath);
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var books = context.Books.MapBookToDto().ToList();

                //VERIFY
                books.Select(x => x.AuthorsOrdered).ToArray()
                    .ShouldEqual(new string[]{ "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

    }
}