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
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filePath, numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.SelectMany(x => x.AuthorsLink.Select(y => y.Author)).Distinct().Count().ShouldEqual(17);
            books.Count(x => x.HasPromotion).ShouldEqual(3);
        }

        [Fact]
        public void TestGenerateBooks20Reviews()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filePath, numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.SelectMany(x => x.Reviews).Count().ShouldEqual(94);
        }

        [Fact]
        public void CheckMainData()
        {
            //SETUP
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var templateBooks = JsonConvert.DeserializeObject<List<BookGenerator.BookData>>(File.ReadAllText(filepath));

            //VERIFY
            templateBooks.Count.ShouldEqual(390);
            templateBooks.Select(x => x.Title).Distinct().Count().ShouldEqual(390);
        }

        [Fact]
        public void TestGenerateBooksFullDataUniqueAuthorLinks()
        {
            //SETUP
            const int numBooks = 600;
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filepath, numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.Count(x => x.AuthorsLink.Select(y => y.Author.Name).Distinct().Count() != x.AuthorsLink.Count()).ShouldEqual(0);
        }

        [Fact]
        public void TestGenerateBooksDistinctTitles()
        {
            //SETUP
            const int numBooks = 600;
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filepath, numBooks).ToList();

            //VERIFY
            books.Count().ShouldEqual(numBooks);
            books.Select(x => x.Title).Distinct().Count().ShouldEqual(390);
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
                var gen = new BookGenerator();
                gen.WriteBatchSize = 10;
                gen.WriteBooks(filePath, numBooks, context, x => {
                    progress.Add(x);
                    return false;
                });

                //VERIFY
                context.Books.Count().ShouldEqual(20);
                progress.ShouldEqual(new List<int>{0, 10, 20});
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
                var gen = new BookGenerator();
                gen.WriteBatchSize = 10;
                gen.WriteBooks(filePath, numBooks, context,
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