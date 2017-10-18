// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Helpers;

namespace test.EfHelpers
{
    public static class MySqlOptions
    {
        /// <summary>
        /// This creates a new and seeded database if not already present, with a name that is unique to the class+method that called it
        /// </summary>
        public static DbContextOptions<EfCoreContext> MySqlClassUniqueDatabaseSeeded4Books<T>(this T testClass)
        {
            var options = MySqlOptionsWithCorrectConnection(testClass);
            EfOptionsHelper.EnsureDatabaseIsCreatedAndSeeded(options, true, false);

            return options;
        }


        public static DbContextOptions<EfCoreContext> MySqlOptionsWithCorrectConnection<T>(this T testClass, string methodName = null)
        {
            var connection = testClass.GetMySqlUniqueDatabaseConnectionString(methodName);

            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseMySql(connection, 
                options => options.MaxBatchSize(1)); //Needed to overcome https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/397);
            return optionsBuilder.Options;
        }

    }
}