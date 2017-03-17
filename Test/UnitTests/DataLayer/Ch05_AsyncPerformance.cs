// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch05_AsyncPerformance
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<EfCoreContext> _options;

        private readonly int _firstBookId;

        public Ch05_AsyncPerformance(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
            using (var context = new EfCoreContext(_options))
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.Books.AddRange(EfTestData.CreateDummyBooks(1000, false, false));
                    context.SaveChanges();
                }
                _firstBookId = context.Books.First().BookId;
            }
        }

        [Fact]
        public async Task SimpleAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));

                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));
                await Task.WhenAll(RunTestAsync(context, 100, "Second access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
                RunTest(context, 100, "Second access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
            }
            //VERIFY
        }


        [Fact]
        public async Task ComplexAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));

                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));
                await Task.WhenAll(RunTestAsync(context, 100, "Second access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
                RunTest(context, 100, "Second access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
            }
            //VERIFY
        }
        
        //--------------------------------------------------------

        private void RunTest(EfCoreContext context, int numCyclesToRun, string testType, Action<EfCoreContext, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context, i + _firstBookId);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double) numCyclesToRun));
        }

        private async Task RunTestAsync(EfCoreContext context, int numCyclesToRun, string testType, Func<EfCoreContext, int, Task> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                await actionToRun(context, i + _firstBookId);//.ConfigureAwait(false);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }
    }
}