// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using test.Helpers;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EfCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch08_ComputedColumn
    {

        [Fact]
        public void TestComputedColumnAddOk()
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
                var entity = new Person{ Name = "Unit Test"};
                entity.SetDateOfBirth(new DateTime(2020, 1,1));
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2020);
            }
        }

        [Fact]
        public void TestComputedColumnUpdateOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter08DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                var entity = new Person { Name = "Unit Test" };
                entity.SetDateOfBirth(new DateTime(2020, 1, 1));
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                entity.SetDateOfBirth(new DateTime(2000, 1, 1));
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2000);
            }
        }


    }
}