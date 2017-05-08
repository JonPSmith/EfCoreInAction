// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EfCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch08_DefaultValues
    {
        [Fact]
        public void TestDefaultConstantValueOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                var logger = new LogDbContext(context);
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test" };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.DateOfBirth.ShouldEqual(new DateTime(2000, 1, 1));
            }
        }

        [Fact]
        public void TestDefaultConstantValueOverriddenOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test", DateOfBirth = new DateTime(2015,2,3)};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.DateOfBirth.ShouldEqual(new DateTime(2015, 2, 3));
            }
        }

        [Fact]
        public void TestDefaultConstantSqlOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test"};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.CreatedOn.Subtract(DateTime.UtcNow).Seconds.ShouldBeInRange(-3,0);
            }
        }

        [Fact]
        public void TestValueGeneratorCalledOnAddOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DefaultTest {Name = "Unit Test"};
                context.Add(entity);

                //VERIFY
                entity.OrderId.StartsWith("Unit Test-").ShouldBeTrue();
            }
        }

        [Fact]
        public void TestValueGeneratorOverriddenOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test", OrderId = "override"};
                context.Add(entity);

                //VERIFY
                entity.OrderId.ShouldEqual("override");
            }
        }
    }
}