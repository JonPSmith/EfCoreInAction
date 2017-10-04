// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using Newtonsoft.Json;
using ServiceLayer.DatabaseServices.Concrete;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch13_BookGenerator
    {
        private readonly ITestOutputHelper _output;

        //public Ch13_BookGenerator(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestGenerateBooks20()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");

            //ATTEMPT
            var gen = new BookGenerator(filePath);
            var books = gen.GenerateBooks(numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.SelectMany(x => x.AuthorsLink.Select(y => y.Author)).Distinct().Count().ShouldEqual(17);
            books.Count(x => x.Promotion != null).ShouldEqual(3);
        }

        [Fact]
        public void CheckMainData()
        {
            //SETUP
            var filePath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var templateBooks = JsonConvert.DeserializeObject<List<BookGenerator.BookData>>(File.ReadAllText(filePath));

            //VERIFY
            templateBooks.Count.ShouldEqual(390);
            templateBooks.Select(x => x.Title).Distinct().Count().ShouldEqual(390);
        }

        [Fact]
        public void TestGenerateBooksFullDataUniqueAuthorLinks()
        {
            //SETUP
            const int numBooks = 600;
            var filePath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator(filePath);
            var books = gen.GenerateBooks(numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.Count(x => x.AuthorsLink.Select(y => y.Author.Name).Distinct().Count() != x.AuthorsLink.Count).ShouldEqual(0);
        }

        [Fact]
        public void TestGenerateBooksNotDistinctTitles()
        {
            //SETUP
            const int numBooks = 600;
            var filePath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator(filePath);
            var books = gen.GenerateBooks(numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.Select(x => x.Title).Distinct().Count().ShouldEqual(390);
        }

        [Fact]
        public void TestGenerateBooksDistinctTitles()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");

            //ATTEMPT
            var gen = new BookGenerator(filePath, true);
            var books = gen.GenerateBooks(numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.Select(x => x.Title).Distinct().Count().ShouldEqual(20);
            books.Count(x => x.Title.Contains("(copy ")).ShouldEqual(10);
        }

        //--------------------------------------------------------------------

        [Fact]
        public void TestWriteBooks20()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var progress = new List<int>();

                //ATTEMPT
                var gen = new BookGenerator(filePath);
                gen.WriteBooks(numBooks, options, x => {
                    progress.Add(x);
                    return false;
                });

                //VERIFY
                context.Books.Count().ShouldEqual(20);
                progress.ShouldEqual(new List<int>{0, 10, 20});
            }
        }

        [Fact]
        public void TestWriteBooks20SecondTime()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var progress = new List<int>();

                //ATTEMPT
                var gen = new BookGenerator(filePath);
                gen.WriteBooks(numBooks, options, x => {
                    progress.Add(x);
                    return false;
                });
                gen.WriteBooks(numBooks, options, x => {
                    progress.Add(x);
                    return false;
                });

                //VERIFY
                context.Books.Count().ShouldEqual(40);
                context.Authors.Count().ShouldEqual(17);
            }
        }

        [Fact]
        public void TestWriteBooks20CancelSecondBatch()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var cancelCount = 0;
                var progress = new List<int>();

                //ATTEMPT
                var gen = new BookGenerator(filePath);
                gen.WriteBooks(numBooks, options,
                    x => {
                        progress.Add(x);
                        return cancelCount++ > 0;
                    });

                //VERIFY
                context.Books.Count().ShouldEqual(10);
                progress.ShouldEqual(new List<int> { 0, 10 });
            }
        }
    }
}