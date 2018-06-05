// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Helpers;

namespace test.EfHelpers
{
    public static class EfOptionsHelper
    {
        /// <summary>
        /// This creates a new and seeded database every time, with a name that is unique to the class+method that called it
        /// </summary>
        public static DbContextOptions<EfCoreContext> NewMethodUniqueDatabaseSeeded4Books<T>(this T testClass,
             [CallerMemberName] string methodName = "")
        {
            var optionsBuilder = SetupOptionsWithCorrectConnection(testClass, methodName);
            EnsureDatabaseIsCreatedAndSeeded(optionsBuilder.Options, true, true);

            return optionsBuilder.Options;
        }

        /// <summary>
        /// This creates a new and seeded database if not already present, with a name that is unique to the class+method that called it
        /// </summary>
        public static DbContextOptions<EfCoreContext> ClassUniqueDatabaseSeeded4Books<T>(this T testClass)
        {
            var optionsBuilder = SetupOptionsWithCorrectConnection(testClass);
            EnsureDatabaseIsCreatedAndSeeded(optionsBuilder.Options, true, false);

            return optionsBuilder.Options;
        }

        /// <summary>
        /// This creates a new and seeded database every time, with a name that is unique to the class that called it
        /// </summary>
        public static DbContextOptions<EfCoreContext> NewClassUniqueDatabaseSeeded4Books<T>(this T testClass)
        {
            var optionsBuilder = SetupOptionsWithCorrectConnection(testClass);
            EnsureDatabaseIsCreatedAndSeeded(optionsBuilder.Options, true, true);

            return optionsBuilder.Options;
        }


        //--------------------------------------------------------------------
        //private methods
        private static DbContextOptionsBuilder<EfCoreContext> SetupOptionsWithCorrectConnection<T>(T testClass, string methodName = null)
        {
            var connection = testClass.GetUniqueDatabaseConnectionString(methodName);
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            return optionsBuilder;
        }

        private static void EnsureDatabaseIsCreatedAndSeeded(DbContextOptions<EfCoreContext> options, bool seedDatabase, bool deleteDatabase)
        {
            using (var context = new EfCoreContext(options))
            {
                if (deleteDatabase)
                    context.Database.EnsureDeleted();

                if (context.Database.EnsureCreated() && seedDatabase)
                    context.SeedDatabaseFourBooks();
            }
        }
    }
}