// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Helpers;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_RawSqlCommands
    {
        private readonly ITestOutputHelper _output;

        private DbContextOptions<EfCoreContext> _options;

        public Ch09_RawSqlCommands(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;

            using (var context = new EfCoreContext(_options))
            {
                if (context.Database.EnsureCreated())
                {
                    context.AddUpdateSqlProcs();
                }
            }
        }




        [Fact]
        public void TestCheckProcExistsOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                context.AddUpdateSqlProcs();

                //VERIFY
                context.EnsureSqlProcsSet().ShouldBeTrue();
            }
        }

        

        
    }
}