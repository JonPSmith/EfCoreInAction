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
    public class Ch08_Sequence
    {

        [Fact]
        public void TestSequenceAddOk()
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
                var entity = new Order();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.OrderNo.ShouldNotEqual(0);
            }
        }

        [Fact]
        public void TestSequenceAddTwiceOk()
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
                var entity1 = new Order();
                var entity2 = new Order();
                context.AddRange(entity1, entity2);
                context.SaveChanges();

                //VERIFY
                entity2.OrderNo.ShouldEqual(entity1.OrderNo + 5);
            }
        }

    }
}