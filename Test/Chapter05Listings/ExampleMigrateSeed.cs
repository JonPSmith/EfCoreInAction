// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;

namespace test.Chapter05Listings
{
    public static class ExampleMigrateSeed
    {
        public static void MigrateAndSeed //#A
            (this EfCoreContext context)  //#A
        {
            if (!context.Database              //#B
                .GetPendingMigrations().Any()) //#B
                return;                        //#B

            context.Database.Migrate();   //#C

            if (context.Books.Any()) return;//#D

            context.Books.AddRange(           //#E
                EfTestData.CreateFourBooks());//#E
            context.SaveChanges();            //#E
        }
        /************************************************
        #A This is an extension method that takes in the application's DbContext
        #B I do a quick check to see if there are any migrations pending and return immediately if there aren't - saves some time
        #C Now I run the EF Core migrate method, which will update the database structure to the format that the entity classes and EF Core configuration says it should be at
        #D If there are existing books I return, as I don't need to add any
        #E TThe database has no books, so I seed it, which in this case I add the default books
         * ************************************************/
    }
}