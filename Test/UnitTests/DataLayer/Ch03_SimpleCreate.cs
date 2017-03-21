// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.Chapter03Listings;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_SimpleCreate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_SimpleCreate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateOneEntry()
        {
            //SETUP
            using (var context = new SimpleDbContext(
                SqliteInMemory.CreateOptions<SimpleDbContext>()))
            {
                context.Database.EnsureCreated();
                var itemToAdd = new ExampleEntity
                {                               
                    MyMessage = "Hello World"   
                };

                //ATTEMPT
                context.Add(itemToAdd); //#A
                context.SaveChanges(); //#B
                /*********************************************************
                #A It use the Add method to add the SingleEntity to the application's DbContext. The DbContext works what table to add it to based on its type of its parameter
                #B It calls the SaveChanges() method from the application's DbContext to update the database
                 * ***********************************************************/

                //VERIFY
                context.SingleEntities.Count()
                    .ShouldEqual(1);          
                itemToAdd.ExampleEntityId      
                    .ShouldNotEqual(0);       
            }
        }

        [Fact]
        public void TestCreateOneEntryWithLogs()
        {
            //SETUP
            var logs = new List<string>();
            using (var context = new SimpleDbContext(
                SqliteInMemory.CreateOptions<SimpleDbContext>()))
            {
                context.Database.EnsureCreated();
                SqliteInMemory.SetupLogging(context, logs);

                var itemToAdd = new ExampleEntity
                {
                    MyMessage = "Hello World"
                };

                //ATTEMPT
                context.SingleEntities.Add(      
                    itemToAdd);                  
                context.SaveChanges();           

                //VERIFY
                context.SingleEntities.Count()   
                    .ShouldEqual(1);
                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestCreateOneEntrySqlServerWithLogs()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<SimpleDbContext>();

            optionsBuilder.UseSqlServer(connection);
            var logs = new List<string>();
            using (var context = new SimpleDbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                SqliteInMemory.SetupLogging(context, logs);

                var itemToAdd = new ExampleEntity
                {
                    MyMessage = "Hello World"
                };

                //ATTEMPT
                context.SingleEntities.Add(
                    itemToAdd);
                context.SaveChanges();

                //VERIFY
                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}