// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using DataLayer.SqlCode;
using Newtonsoft.Json;
using ServiceLayer.BookServices.QueryObjects;
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
        public void TestBookGenerator20()
        {
            //SETUP
            const int numBooks = 20;
            var filePath = TestFileHelpers.GetTestFileFilePath("Manning books - only 10.json");

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filePath, numBooks);

            //VERIFY
            books.Count.ShouldEqual(numBooks);
            books.SelectMany(x => x.AuthorsLink.Select(y => y.Author)).Distinct().Count().ShouldEqual(17);
            books.SelectMany(x => x.AuthorsLink).Distinct().Count().ShouldEqual(books.SelectMany(x => x.AuthorsLink).Count());
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
        public void TestBookGeneratorFullDataUniqueAuthorLinks()
        {
            //SETUP
            const int numBooks = 600;
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filepath, numBooks);

            //VERIFY
            books.Count.ShouldEqual(numBooks);
            books.Count(x => x.AuthorsLink.Select(y => y.Author.Name).Distinct().Count() != x.AuthorsLink.Count).ShouldEqual(0);
        }

        [Fact]
        public void TestBookGeneratorDistinctTitles()
        {
            //SETUP
            const int numBooks = 600;
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\", SetupHelpers.SeedFileSubDirectory,
                SetupHelpers.TemplateFileName);

            //ATTEMPT
            var gen = new BookGenerator();
            var books = gen.GenerateBooks(filepath, numBooks);

            //VERIFY
            books.Count.ShouldEqual(numBooks);
            books.Select(x => x.Title).Distinct().Count().ShouldEqual(390);
        }
    }
}