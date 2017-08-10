// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using test.Attributes;
using test.EfHelpers;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_AddRangePerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch12_AddRangePerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void AddAndAddRangeCompare()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var entity = EfTestData.CreateDummyBookOneAuthor();
                var entities = new List<Book>();
                for (int i = 0; i < 100; i++)
                {
                    entities.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                RunTest(context, 1, "AddRange 100", c => c.AddRange(entities));
                RunTest(context, 100, "Add 100", c => c.Add(entity));
                var entity2 = EfTestData.CreateDummyBookOneAuthor();
                var entities2 = new List<Book>();
                for (int i = 0; i < 1000; i++)
                {
                    entities2.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                RunTest(context, 1000, "Add 1000", c => c.Add(entity2));
                RunTest(context, 1, "AddRange 1000", c => c.AddRange(entities2));
            }
        }

        [RunnableInDebugOnly]
        public void AddAndAddRangeWithSaveChangesCompare()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                //var logs = new List<string>();
                context.Database.EnsureCreated();
                //SqliteInMemory.SetupLogging(context, logs);
                context.Add(EfTestData.CreateDummyBookOneAuthor());
                context.SaveChanges();

                for (int i = 0; i < 100; i++)
                {
                    context.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                RunTest(context, 1, "SaveChanges after Add", c => c.SaveChanges());
                var entities = new List<Book>();
                for (int i = 0; i < 100; i++)
                {
                    entities.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                context.AddRange(entities);
                RunTest(context, 1, "SaveChanges after AddRange", c => c.SaveChanges());

                for (int i = 0; i < 100; i++)
                {
                    context.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                RunTest(context, 1, "SaveChanges after Add", c => c.SaveChanges());

                var entities2 = new List<Book>();
                for (int i = 0; i < 100; i++)
                {
                    entities2.Add(EfTestData.CreateDummyBookOneAuthor());
                }
                context.AddRange(entities2);
                RunTest(context, 1, "SaveChanges after AddRange", c => c.SaveChanges());
                //foreach (var log in logs)
                //{
                //    _output.WriteLine(log);
                //}
            }
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