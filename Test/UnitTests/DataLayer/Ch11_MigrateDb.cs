// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter10Listings.EfCode;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch11_MigrateDb
    {
        private readonly ITestOutputHelper _output;

        public Ch11_MigrateDb(ITestOutputHelper output)
        {
            _output = output;
        }

        //You can only run this test before the migration!!
        //[Fact]
        //public void WriteToInitialDatabase()
        //{
        //    //SETUP
        //    using (var context = new Chapter11MigrateDb())
        //    {
        //        //ATTEMPT
        //        var cAndD = new List<CustomerAndAddresses>
        //        {
        //            new CustomerAndAddresses {Name = "Joe", Address = "10 a street"},
        //            new CustomerAndAddresses {Name = "Jane", Address = "99 some street"},
        //            new CustomerAndAddresses {Name = "Jim", Address = "10 my street"}
        //        };
        //        context.AddRange(cAndD);
        //        context.SaveChanges();

        //        //VERIFY
        //        context.CustomerAndAddresses.Count().ShouldEqual(3);
        //    }
        //}

        
    }
}
