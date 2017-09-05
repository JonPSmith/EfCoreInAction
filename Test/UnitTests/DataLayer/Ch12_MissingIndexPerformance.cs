// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EfCode;
using Xunit;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_MissingIndexPerformance
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<Chapter12DbContext> _options;
        private readonly List<IndexClass> _entities;

        public Ch12_MissingIndexPerformance(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;

            const int numRows = 1000;

            using (var context = new Chapter12DbContext(_options))
            {
                context.Database.EnsureCreated();
                if (!context.IndexClasses.Any())
                {
                    var entities = new List<IndexClass>();
                    for (int i = 0; i < numRows; i++)
                    {
                        var uniqueString = Guid.NewGuid().ToString();
                        entities.Add(new IndexClass
                        {
                            NoIndex = uniqueString,
                            WithIndex = uniqueString
                        });
                    }
                    context.AddRange(entities);
                    context.SaveChanges();
                }
                _entities = context.IndexClasses.ToList();
            }
        }

        [RunnableInDebugOnly]
        public void SortNoIndexPerformance()
        {
            //SETUP
            using (var context = new Chapter12DbContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, SortNoIndex:", (c, i) => c.IndexClasses.OrderBy(x => x.NoIndex).ToList());
                RunTest(context, 1, "Second access, SortNoIndex", (c, i) => c.IndexClasses.OrderBy(x => x.NoIndex).ToList());
                RunTest(context, 100, "Multi access, SortNoIndex", (c, i) => c.IndexClasses.OrderBy(x => x.NoIndex).ToList());
            }
        }

        [RunnableInDebugOnly]
        public void SortWithIndexPerformance()
        {
            //SETUP
            using (var context = new Chapter12DbContext(_options))
            {
                var logger = new LogDbContext(context);
                //ATTEMPT
                RunTest(context, 1, "First access, SortWithIndex:", (c, i) => c.IndexClasses.OrderBy(x => x.WithIndex).ToList());
                var oneLogs = logger.Logs;
                RunTest(context, 1, "Second access, SortWithIndex", (c, i) => c.IndexClasses.OrderBy(x => x.WithIndex).ToList());
                RunTest(context, 100, "Multi access, SortWithIndex", (c, i) => c.IndexClasses.OrderBy(x => x.WithIndex).ToList());
                foreach (var log in oneLogs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [RunnableInDebugOnly]
        public void SearchNoIndexPerformance()
        {
            //SETUP
            using (var context = new Chapter12DbContext(_options))
            {
                var logger = new LogDbContext(context);
                //ATTEMPT
                RunTest(context, 1, "First access, SearchNoIndex:", (c, i) => c.IndexClasses.First(x => x.NoIndex == _entities[i].NoIndex));
                var oneLogs = logger.Logs;
                RunTest(context, 1, "Second access, SearchNoIndex", (c, i) => c.IndexClasses.First(x => x.NoIndex == _entities[i].NoIndex));
                RunTest(context, 100, "Multi access, SearchNoIndex", (c, i) => c.IndexClasses.First(x => x.NoIndex == _entities[i].NoIndex));
                foreach (var log in oneLogs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [RunnableInDebugOnly]
        public void SearchWithIndexPerformance()
        {
            //SETUP
            using (var context = new Chapter12DbContext(_options))
            {

                //ATTEMPT
                RunTest(context, 1, "First access, SearchWithIndex:", (c, i) => c.IndexClasses.First(x => x.WithIndex == _entities[i].WithIndex));
                RunTest(context, 1, "Second access, SearchWithIndex", (c, i) => c.IndexClasses.First(x => x.WithIndex == _entities[i].WithIndex));
                RunTest(context, 100, "Multi access, SearchWithIndex", (c, i) => c.IndexClasses.First(x => x.WithIndex == _entities[i].WithIndex));
            }
        }

        //--------------------------------------------------------

        private void RunTest(Chapter12DbContext context, int numCyclesToRun, string testType, Action<Chapter12DbContext, int> actionToRun)
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
                timer.ElapsedMilliseconds / ((double) numCyclesToRun));
        }
    }
}