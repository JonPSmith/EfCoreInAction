// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseServices.Concrete;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_LikeCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch02_LikeCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLike()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooksBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "Book00_5%"))
                    .ToList();


                //VERIFY
                books.Count.ShouldEqual(4);
                foreach (var log in sqlite.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void FindBooksWithCSharpInTheirTitle()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabase(TestFileHelpers.GetSolutionDirectory() + @"\EfCoreInAction\wwwroot\");
                sqlite.ClearLogs();

                //ATTEMPT
                var bookTitles = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "%C#%"))
                    .Select(p => p.Title)
                    .ToList();


                //VERIFY
                bookTitles.Count.ShouldEqual(5);
                foreach (var title in bookTitles)
                {
                    _output.WriteLine(title);
                }
            }
        }

    }
}