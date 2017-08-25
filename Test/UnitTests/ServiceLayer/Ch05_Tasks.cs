// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.Chapter05Listings;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch05_Tasks
    {
        //Must be run on it own to check things
        [RunnableInDebugOnly]
        public async Task TwoTasksSameDbContextBad()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var task1 = MyTask(context);
                var task2 = MyTask(context);

                var exceptionRaised = false;
                try
                {
                    await Task.WhenAll(task1, task2);
                }
                catch (Exception e)
                {
                    e.ShouldBeType<InvalidOperationException>();
                    e.Message.ShouldEqual("A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe.");
                    exceptionRaised = true;
                }

                //VERIFY
                exceptionRaised.ShouldBeTrue();
            }
        }

        //Must be run on it own to check things
        [RunnableInDebugOnly]
        public async Task TwoTasksDifferentDbContextOk()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            using (var context1 = new EfCoreContext(options))
            using (var context2 = new EfCoreContext(options))
            {

                //ATTEMPT
                var task1 = MyTask(context1);
                var task2 = MyTask(context2);

                await Task.WhenAll(task1, task2);
            }
        }

        private async Task<int> MyTask(EfCoreContext context)
        {
            await Task.Delay(10);
            return context.Books.Count();
        }
    }
}