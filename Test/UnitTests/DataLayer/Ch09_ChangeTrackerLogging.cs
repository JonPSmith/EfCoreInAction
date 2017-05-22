// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_ChangeTrackerLogging
    {
        private readonly ITestOutputHelper _output;

        public Ch09_ChangeTrackerLogging(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSetWhenWhereNotCalledOk()
        {
            //SETUP
            var entity = new LoggedEntity();
 
            //ATTEMPT

            //VERIFY
            entity.CreatedBy.ShouldBeNull();
            entity.CreatedOn.ShouldEqual(new DateTime());
            entity.UpdatedBy.ShouldBeNull();
            entity.UpdatedOn.ShouldEqual(new DateTime());
        }

        [Fact]
        public void TestSetWhenWhereAddOk()
        {
            //SETUP
            var entity = new LoggedEntity();

            //ATTEMPT
            entity.SetWhenWhere(() => "Test User", true);

            //VERIFY
            entity.CreatedBy.ShouldEqual("Test User");
            entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5,0);
            entity.UpdatedBy.ShouldEqual("Test User");
            entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

        [Fact]
        public void TestSetWhenWhereUpdateOk()
        {
            //SETUP
            var entity = new LoggedEntity();

            //ATTEMPT
            entity.SetWhenWhere(() => "Test User", false);

            //VERIFY
            entity.CreatedBy.ShouldBeNull();
            entity.CreatedOn.ShouldEqual(new DateTime());
            entity.UpdatedBy.ShouldEqual("Test User");
            entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

        [Fact]
        public void TestLoggedEntityAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options, () => "Test User"))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new LoggedEntity();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.CreatedBy.ShouldEqual("Test User");
                entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
                entity.UpdatedBy.ShouldEqual("Test User");
                entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
            }
        }

        [Fact]
        public void TestLoggedEntityUpdateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options, () => "Test User1"))
            {
                context.Database.EnsureCreated();

                var entity = new LoggedEntity();
                context.Add(entity);
                context.SaveChanges();
                Thread.Sleep(1000);
            }
            using (var context = new Chapter09DbContext(options, () => "Test User2"))
            {
                    //ATTEMPT
                var entity = context.LoggedEntities.First();
                entity.MyString = "New Value";
                context.SaveChanges();

                //VERIFY
                entity.CreatedBy.ShouldEqual("Test User1");
                entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-1.5, -0.5);
                entity.UpdatedBy.ShouldEqual("Test User2");
                entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
            }
        }

    }
}
