// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_ChangeTrackingPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch09_ChangeTrackingPerformance(ITestOutputHelper output)
        {
            _output = output;
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void TestSaveChangesPerformanceBooksOk(int numBooks)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);
            }
            using (var context = new EfCoreContext(options))
            {
                var books = context.Books
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .ToList();

                //ATTEMPT
                var timer = new Stopwatch();
                timer.Start();
                context.SaveChanges();
                timer.Stop();

                //VERIFY
                _output.WriteLine("#{0:####0} books: total time = {1} ms ", numBooks,
                    timer.ElapsedMilliseconds);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestSaveChangesPerformanceMyEntityOk(int numEntities)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entities = new List<MyEntity>();
                for (int i = 0; i < numEntities; i++)
                {
                    var entity = new MyEntity();
                    entities.Add(entity);
                }
                context.AddRange(entities);
                context.SaveChanges();

            }
            using (var context = new Chapter09DbContext(options))
            {
                var entities = context.MyEntities.ToList();
                entities.Count.ShouldEqual(numEntities);

                //ATTEMPT
                var timer = new Stopwatch();
                timer.Start();
                context.SaveChanges();
                timer.Stop();

                //VERIFY
                _output.WriteLine("#{0:####0} entities: total time = {1} ms ", numEntities,
                    timer.ElapsedMilliseconds);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestSaveChangesPerformanceNotifyEntityOk(int numEntities)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entities = new List<NotifyEntity>();
                for (int i = 0; i < numEntities; i++)
                {
                    var entity = new NotifyEntity();
                    entities.Add(entity);
                }
                context.AddRange(entities);
                context.SaveChanges();

            }
            using (var context = new Chapter09DbContext(options))
            {
                var entities = context.Notify.ToList();
                entities.Count.ShouldEqual(numEntities);

                //ATTEMPT
                var timer = new Stopwatch();
                timer.Start();
                context.SaveChanges();
                timer.Stop();

                //VERIFY
                _output.WriteLine("#{0:####0} entities: total time = {1} ms ", numEntities,
                    timer.ElapsedMilliseconds);
            }
        }
    }
}
