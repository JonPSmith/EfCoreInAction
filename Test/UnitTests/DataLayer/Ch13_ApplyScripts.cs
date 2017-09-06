// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using DataLayer.EfCode;
using DataLayer.SqlCode;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch13_ApplyScripts
    {
        private readonly ITestOutputHelper _output;

        public Ch13_ApplyScripts(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCorrectPathToSqlFile()
        {
            //SETUP

            //ATTEMPT
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\",
                UdfDefinitions.SqlScriptName);

            //VERIFY
            File.ReadAllLines(filepath).First().ShouldEqual("-- SQL script file to add SQL code to improve performance");

        }

        [Fact]
        public void TestApplySqlScriptFileToDatabase()
        {
            //SETUP
            var options = this.ClassUniqueDatabaseSeeded4Books();
            var filepath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"EfCoreInAction\wwwroot\",
                UdfDefinitions.SqlScriptName);

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);

                //ATTEMPT
                context.ExecuteScriptFileInTransaction(filepath);

                //VERIFY
                context.Books.Select(x => UdfDefinitions.AuthorsStringUdf(x.BookId)).ToArray()
                    .ShouldEqual(new string[]{ "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

    }
}