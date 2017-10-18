// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using DataLayer.SqlCode;
using ServiceLayer.BookServices;
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

        [Fact]
        public void TestBookListDtoImprovedCheckVotes()
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
                books.Select(x => x.ReviewsAverageVotes).ToArray()
                    .ShouldEqual(new decimal?[] { null, null, null, 5 });
                books.Select(x => x.ReviewsCount).ToArray()
                    .ShouldEqual(new int[] { 0, 0, 0, 2 });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestBookListDtoImprovedCheckActualPrice()
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
                books.Select(x => x.ActualPrice).ToArray()
                    .ShouldEqual(new decimal[] { 40, 53, 56, 219 });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestBookListDtoImprovedOrderByVotes()
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
                var books = context.Books.MapBookToDto().OrderBooksBy(OrderByOptions.ByVotes).ToList();

                //VERIFY
                books.Select(x => x.AuthorsOrdered).ToArray()
                    .ShouldEqual(new string[] { "Future Person", "Martin Fowler", "Martin Fowler", "Eric Evans" });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        private class SimpleDto
        {
            public double? ReviewsAverageVotes { get; set; }
            public string AuthorsOrdered { get; set; }
        }

        [Fact] public void TestUdfExceptionSimpleDto()
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
                var books = context.Books.Select(p => new SimpleDto
                {
                    AuthorsOrdered = UdfDefinitions.AuthorsStringUdf(p.BookId)
                }).OrderBy(x => x.ReviewsAverageVotes).ToList();

                //VERIFY
                books.Select(x => x.AuthorsOrdered).ToArray()
                    .ShouldEqual(new string[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

    }
}