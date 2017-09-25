// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch13_LocalViewListener
    {
        private readonly ITestOutputHelper _output;

        public Ch13_LocalViewListener(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestILocalViewListener()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var bookTracked = context.Books.First();

                //ATTEMPT
                var eventLog = new List<string>();
                var events = context.GetService<ILocalViewListener>();
                events.RegisterView((entry, state) =>
                {
                    eventLog.Add($"{entry.Entity.GetType().Name} state = {state}");
                });
                bookTracked.Title = Guid.NewGuid().ToString();
                context.SaveChanges();

                //VERIFY
                eventLog.ShouldEqual(new List<string>{"Book state = Unchanged", "Book state = Modified"});
            }
        }

    }
}