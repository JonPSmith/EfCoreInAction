// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch06_ShadowProperties
    {
        //private readonly ITestOutputHelper _output;

        //public Ch06_ShadowProperties(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestShadowPropertyExistsOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT

                    //VERIFY
                    context.GetColumnName<MyEntityClass>("UpdatedOn").ShouldEqual("UpdatedOn");
                }
            }
        }

        [Fact]
        public void SetShadowPropertyOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new MyEntityClass  //#A
                        { InDatabaseProp = "Hello"};//#A
                    context.Add(entity); //#B
                    context.Entry(entity) //#C
                        .Property("UpdatedOn").CurrentValue //#D
                            = DateTime.Now; //#E
                    context.SaveChanges(); //#F
                    /************************************************
                    #A I create an entity class
                    #B ... and add it to the context. That means it is now tracked
                    #C I then get the EntityEntry from the tracked entity data
                    #D Using the Property method I can get the shadow property with read/write access
                    #E Then I set that property to the value I want
                    #F Finally I call SaveChanges to save the MyEntityClass instance, with its normal and shadow property values, to the database
                     * *********************************************/
                    //VERIFY
                }
            }
        }

        [Fact]
        public void ReadShadowPropertyOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var timeNow = DateTime.Now;
                    var entity = new MyEntityClass
                    { InDatabaseProp = "Hello" };
                    context.Add(entity); 
                    context.Entry(entity).Property("UpdatedOn").CurrentValue = timeNow; 
                    context.SaveChanges(); 


                    //VERIFY
                    var readEntity = context.MyEntities.First();
                    context.Entry(readEntity).Property("UpdatedOn").CurrentValue.ShouldEqual(timeNow);
                }
            }
        }

        [Fact]
        public void ReadShadowPropertyAsNoTrackedOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var timeNow = DateTime.Now;
                    var entity = new MyEntityClass
                    { InDatabaseProp = "Hello" };
                    context.Add(entity);
                    context.Entry(entity).Property("UpdatedOn").CurrentValue = timeNow;
                    context.SaveChanges();


                    //VERIFY
                    var readEntity = context.MyEntities.AsNoTracking().First();
                    context.Entry(readEntity).Property("UpdatedOn").CurrentValue.ShouldEqual(new DateTime());
                }
            }
        }



    }
}