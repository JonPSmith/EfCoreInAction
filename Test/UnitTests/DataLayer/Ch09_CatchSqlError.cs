// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Helpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_CatchSqlError 
    {
        private readonly ITestOutputHelper _output;
        private DbContextOptions<Chapter09DbContext> _options;

        public Ch09_CatchSqlError(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter09DbContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
        }

        [Fact]
        public void TestUniqueConstraintException()
        {
            //SETUP
            using (var context = new Chapter09DbContext(_options))
            {
                context.Database.EnsureCreated();
                var unique = Guid.NewGuid().ToString();
                context.Add(new MyUnique() { UniqueString = unique });
                context.SaveChanges();

                //ATTEMPT
                context.Add(new MyUnique() { UniqueString = unique });
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.ShouldBeType<SqlException>();
                ex.InnerException.Message
                    .StartsWith("Cannot insert duplicate key row in object 'dbo.MyUnique' with unique index 'UniqueError_UniqueString'. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestHandleUniqueConstraintOk()
        {
            //SETUP
            using (var context = new Chapter09DbContext(_options))
            {
                context.Database.EnsureCreated();
                var checker = new SaveChangesWithSqlCheck(
                    context, new Dictionary<int, FormatSqlException>
                {
                    [2601] = SqlExceptionErrorHandlers.UniqueConstraintExceptionHandler
                });
                var unique = Guid.NewGuid().ToString();

                //ATTEMPT
                context.Add(new MyUnique() { UniqueString = unique });
                var error1 = checker.SaveChangesWithChecking();
                error1.ShouldBeNull();
                context.Add(new MyUnique() { UniqueString = unique });
                var error2 = checker.SaveChangesWithChecking();

                //VERIFY
                error2.ShouldNotBeNull();
                error2.ErrorMessage.StartsWith("Cannot have a duplicate UniqueString in MyUnique. Duplicate value was ").ShouldBeTrue();
            }
        }

    }
}
