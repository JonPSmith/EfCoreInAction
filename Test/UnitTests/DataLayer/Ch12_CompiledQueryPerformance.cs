// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.Attributes;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_CompiledQueryPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch12_CompiledQueryPerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        private static Func<EfCoreContext, int, Book> _compliedQueryComplex =
            EF.CompileQuery((EfCoreContext context, int i) =>
                context.Books
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Where(x => !x.SoftDeleted)
                    .OrderBy(x => x.PublishedOn)
                    .Skip(i)
                    .FirstOrDefault());

        private void RunNonCompiledQueryComplex(EfCoreContext context, int i)
        {
            var books = context.Books
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Reviews)
                .Where(x => !x.SoftDeleted)
                .OrderBy(x => x.PublishedOn)
                .Skip(i)
                .FirstOrDefault();
        }

        private static Func<EfCoreContext, int, Book> //#A
            _compliedQuerySimple =                    //#A
            EF.CompileQuery( //#B
                (EfCoreContext context, int i) => //#B
                context.Books//#D
                    .Skip(i) //#D
                    .First() //#D
                );
        /*************************************************
        #A You need to define a static function to hold your complied query. In this case I take in the application's DbContext, an int parameter, and the return type
        #B The EF.CompileQuery expects: a) a DbContext, b) one or two parameters for you to use in your query, c) the returned result, either a entity class or IEnumerable<TEntity>
        #C Now you define the query to want to hold as compiled
         * *************************************************/

        private void RunNonCompiledQuerySimple (EfCoreContext context, int i)
        {
            var book = context.Books.Skip(i)
                .First();
        }

        private static Func<EfCoreContext, IEnumerable<Book>> _compliedQueryEnumerable =
            EF.CompileQuery((EfCoreContext context) =>
                context.Books);

        [RunnableInDebugOnly]
        public void QueryNonCompiledComplex()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, NonCompiled:", RunNonCompiledQueryComplex);
                RunTest(context, 1, "Second access, NonCompiled:", RunNonCompiledQueryComplex);
                RunTest(context, 100, "Multi access, NonCompiled:", RunNonCompiledQueryComplex);
            }
        }

        [RunnableInDebugOnly]
        public void QueryCompiledComplex()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, Compiled:", (c,i) =>_compliedQueryComplex(c,i));
                RunTest(context, 1, "Second access, Compiled:", (c, i) => _compliedQueryComplex(c, i));
                RunTest(context, 100, "Multi access, Compiled:", (c, i) => _compliedQueryComplex(c, i));
            }
        }

        [RunnableInDebugOnly]
        public void QueryNonCompiledSimple()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, NonCompiled:", RunNonCompiledQuerySimple);
                RunTest(context, 1, "Second access, NonCompiled:", RunNonCompiledQuerySimple);
                RunTest(context, 100, "Multi access, NonCompiled:", RunNonCompiledQuerySimple);
            }
        }

        [RunnableInDebugOnly]
        public void QueryCompiledSimple()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
                RunTest(context, 1, "Second access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
                RunTest(context, 100, "Multi access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
            }
        }

        [Fact]
        public void TestEnumerableCompiledQuery()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();

                var logIt = new LogDbContext(context);
                var logger = context.GetService<ILoggerFactory>().CreateLogger("TEST");

                //ATTEMPT
                logger.LogInformation("Before compiled query-----------------------");
                var result = _compliedQueryEnumerable(context);
                logger.LogInformation("After compiled query------------------------");
                var list = result.ToList();
                logger.LogInformation("After TOLIST------------------------");

                //VERIFY
                logIt.Logs.Count.ShouldEqual(4);
                //This proves that the query has not been executed until the ToList is applied to the IEnumerable returned 
                logIt.Logs[2].StartsWith("Information: Executed DbCommand ").ShouldBeTrue();
            }
        }

        //--------------------------------------------------------

        private void RunTest(EfCoreContext context, int numCyclesToRun, string testType, Action<EfCoreContext, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context, i);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }

    }
}