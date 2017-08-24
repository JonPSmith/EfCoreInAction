// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DatabaseServices.Concrete;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_StringSearch
    {
        private readonly ITestOutputHelper _output;

        public Ch02_StringSearch(ITestOutputHelper output)
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
                context.SeedDatabaseDummyBooks(40);
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
        public void TestLikeLowerCase()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "book00_5%"))
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
        public void TestStartsWith()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.StartsWith("Book001"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(10);
                foreach (var log in sqlite.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestEndsWith()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.EndsWith("1 Title"))
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
        public void FindBooksWithCSharpInTheirTitleContains()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabase(TestFileHelpers.GetSolutionDirectory() + @"\EfCoreInAction\wwwroot\");
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.Contains("C#"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(5);
                foreach (var log in sqlite.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void FindBooksWithCSharpInTheirTitleLike()
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

        //Non-database run commands

        [Fact]
        public void TestIndexOf()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.IndexOf("Book001") == 0)
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(10);
                foreach (var log in sqlite.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestSubstringOf()
        {
            //SETUP
            var sqlite = new SqliteInMemory();

            using (var context = sqlite.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(40);
                sqlite.ClearLogs();

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.Substring(0,4) == "Book")
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(40);
                foreach (var log in sqlite.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}