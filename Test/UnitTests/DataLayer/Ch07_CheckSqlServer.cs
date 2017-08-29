// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_CheckSqlServer
    {
        private readonly ITestOutputHelper _output;

        public Ch07_CheckSqlServer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateSqlServerDbOk()
        {
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();
            optionsBuilder.UseSqlServer(connection);

            //SETUP
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                //ATTEMPT
                context.Database.EnsureCreated();

                //VERIFY
            }
        }

        [Fact]
        public void TestDeleteDependentDefaultOk()
        {
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();
            optionsBuilder.UseSqlServer(connection);

            //SETUP
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                var logIt = new LogDbContext(context);
                context.Database.EnsureCreated();

                var numPrincipal = context.DeletePrincipals.Count();
                var numDependent = context.Set<DeleteDependentDefault>().Count();

                var entity = new DeletePrincipal {DependentDefault = new DeleteDependentDefault()};
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                entity.DependentDefault.DeletePrincipalId.ShouldBeNull();
                context.DeletePrincipals.Count().ShouldEqual(numPrincipal);
                context.Set<DeleteDependentDefault>().Count().ShouldEqual(numDependent+1);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}