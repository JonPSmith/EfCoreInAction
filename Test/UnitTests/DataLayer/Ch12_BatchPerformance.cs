// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_BatchPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch12_BatchPerformance(ITestOutputHelper output)
        {
            _output = output;
        }


        [RunnableInDebugOnly]
        public void BatchAddCompare()
        {
            //SETUP
            var options = this.ClassUniqueDatabaseSeeded4Books();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var logs = new List<string>();
                context.Database.EnsureCreated();
                SqliteInMemory.SetupLogging(context, logs);
                var logger = context.GetService<ILoggerFactory>().CreateLogger("TEST");

                context.Add(CreateBookEntity());
                context.SaveChanges();
                const int numSaved = 100;
                logger.LogInformation("PART1 111111111111111111111");
                var entities = new List<Book>();
                for (int i = 0; i < numSaved; i++)
                {
                    entities.Add(CreateBookEntity());
                }
                RunTest(context, 1, $"Add {numSaved} then save", c =>
                {
                    c.AddRange(entities);
                    c.SaveChanges();
                });
                logger.LogInformation("PART2 222222222222222222222222222");
                RunTest(context, numSaved, "Add 1 then save", c =>
                {
                    c.Add(CreateBookEntity());
                    c.SaveChanges();
                });
                logger.LogInformation("PART3333333333333333333333333333333");
                var entities2 = new List<Book>();
                for (int i = 0; i < numSaved; i++)
                {
                    entities2.Add(CreateBookEntity());
                }
                RunTest(context, 1, $"Add {numSaved} then save", c =>
                {
                    c.AddRange(entities2);
                    c.SaveChanges();
                });
                logger.LogInformation("PART44444444444444444444444444444");
                RunTest(context, numSaved, "Add 1 then save", c =>
                {
                    c.Add(CreateBookEntity());
                    c.SaveChanges();
                });

                //foreach (var log in logs)
                //{
                //    _output.WriteLine(log);
                //}
            }
        }

        private Book CreateBookEntity()
        {
            return new Book
            {
                Title = Guid.NewGuid().ToString(),
                Price = 10,
                PublishedOn = new DateTime(2000,1,1)
            };
        }

        private void RunTest(EfCoreContext context, int numCyclesToRun, string testType, Action<EfCoreContext> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }
    }
}