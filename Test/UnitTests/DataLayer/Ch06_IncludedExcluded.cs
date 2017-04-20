// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch06_IncludedExcluded
    {
        //private readonly ITestOutputHelper _output;

        //public Ch06_ShadowProperties(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestIncludedPropertiesOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //var logs = new List<string>();
                    //SqliteInMemory.SetupLogging(context, logs);
                    //context.Database.EnsureCreated();

                    //ATTEMPT
                    var props = context.GetProperties<MyEntityClass>().Select(x => x.Name).ToList();

                    //VERIFY
                    props.ShouldEqual(new List<string>{ "MyEntityClassId", "InDatabaseProp", "InternalSet", "PrivateSet", "UpdatedOn" });
                }
            }
        }

        

    }
}