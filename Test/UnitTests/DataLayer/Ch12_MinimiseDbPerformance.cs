// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_MinimiseDbPerformance
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<EfCoreContext> _options;

        private readonly int _firstBookId;

        public Ch12_MinimiseDbPerformance(ITestOutputHelper output)
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
                    context.Books.AddRange(EfTestData.CreateDummyBooks(100, false, false));
                    context.SaveChanges();
                }
                _firstBookId = context.Books.First().BookId;
            }
        }

        [RunnableInDebugOnly]
        public void SelectPerformance()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var logger = new LogDbContext(context);
                //ATTEMPT
                RunTest(context, 1, "First access, SelectLoad:", SelectLoad);
                var oneLogs = logger.Logs;
                RunTest(context, 1, "Second access, SelectLoad", SelectLoad);
                RunTest(context, 100, "Multi access, SelectLoad", SelectLoad);

                //VERIFY
                foreach (var log in oneLogs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [RunnableInDebugOnly]
        public void EagerPerformance()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var logger = new LogDbContext(context);
                //ATTEMPT
                RunTest(context, 1, "First access, EagerLoad:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));
                var oneLogs = logger.Logs;
                RunTest(context, 1, "Second access, EagerLoad:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));
                RunTest(context, 100, "Multi access, EagerLoad:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));

                //VERIFY
                foreach (var log in oneLogs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [RunnableInDebugOnly]
        public void ExplictPerformance()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var logger = new LogDbContext(context);
                //ATTEMPT
                RunTest(context, 1, "First access, MultipleSmall:", MultipleSmall);
                var oneLogs = logger.Logs;
                RunTest(context, 1, "Second access, MultipleSmall:", MultipleSmall);
                RunTest(context, 10, "Multi access, MultipleSmall:", MultipleSmall);

                //VERIFY
                foreach (var log in oneLogs)
                {
                    _output.WriteLine(log);
                }
            }
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

        private void MultipleSmall(EfCoreContext context, int id)
        {
            var book = context.Books.Single(x => x.BookId == id);
            context.Entry(book).Collection(c => c.AuthorsLink).Load();
            foreach (var authorLink in book.AuthorsLink)
            {                                        
                context.Entry(authorLink)            
                    .Reference(r => r.Author).Load();
            }
            context.Entry(book).Collection(c => c.Reviews).Load();
            context.Entry(book).Reference(r => r.Promotion).Load();
        }

        private void SelectLoad(EfCoreContext context, int id)
        {
            var book = context.Books
                .Select(p => new
                {
                    p.BookId,
                    p.Description,
                    p.Publisher,
                    p.PublishedOn,
                    p.Price,
                    p.ImageUrl,
                    NewPrice = p.Promotion == null ? null : (decimal?)p.Promotion.NewPrice,
                    PromotionalText = p.Promotion == null ? null : p.Promotion.PromotionalText,
                    ReviewsCount = p.Reviews.Count,
                    ReviewsVotes = p.Reviews.Select(x => x.NumStars).ToList(),
                    Authors = p.AuthorsLink.OrderBy(x => x.Order).Select(x => x.Author).ToList()
                })
                .Single(x => x.BookId == id);
        }
    }
}