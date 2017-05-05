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
    public class Ch08_InMemory
    {
        //standard localdb is 2014, not 2016, so in-memory is not supported
        //[Fact]
        //public void TestAddOneOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter08DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter08DbContext(optionsBuilder.Options))
        //    {
        //        context.Database.EnsureCreated();

        //        //ATTEMPT
        //        var entity = new InMemoryTest {TestCode = "1"};
        //        context.Add(entity);
        //        context.SaveChanges();

        //        //VERIFY
        //        entity.Id.ShouldNotEqual(0);
        //    }
        //}

    }
}