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
        public void TestSetWhenNotCalledOk()
        {
            //SETUP
            var entity = new AutoWhenEntity();
 
            //ATTEMPT

            //VERIFY
            entity.CreatedOn.ShouldEqual(new DateTime());
            entity.UpdatedOn.ShouldEqual(new DateTime());
        }

        [Fact]
        public void TestSetWhenAddOk()
        {
            //SETUP
            var entity = new AutoWhenEntity();

            //ATTEMPT
            entity.SetWhen(true);

            //VERIFY
            entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5,0);
            entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

        [Fact]
        public void TestSetWhenUpdateOk()
        {
            //SETUP
            var entity = new AutoWhenEntity();

            //ATTEMPT
            entity.SetWhen(false);

            //VERIFY
            entity.CreatedOn.ShouldEqual(new DateTime());
            entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

        [Fact]
        public void TestAutoWhenEntityAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new AutoWhenEntity();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
                entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
            }
        }

        [Fact]
        public void TestAutoWhenEntityUpdateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new AutoWhenEntity();
                context.Add(entity);
                context.SaveChanges();
                Thread.Sleep(1000);
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.LoggedEntities.First();
                entity.MyString = "New Value";
                context.SaveChanges();

                //VERIFY

                entity.CreatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-1.5, -0.5);
                entity.UpdatedOn.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
            }
        }

    }
}
