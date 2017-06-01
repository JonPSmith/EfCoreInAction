// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.Helpers;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch10_CatchSqlError 
    {
        private readonly ITestOutputHelper _output;
        private DbContextOptions<Chapter10DbContext> _options;

        public Ch10_CatchSqlError(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter10DbContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
        }

        [Fact]
        public void TestUniqueConstraintException()
        {
            //SETUP
            using (var context = new Chapter10DbContext(_options))
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
                    .StartsWith("Cannot insert duplicate key row in object 'dbo.MyUnique' with unique index 'UniqueError_MyUnique_UniqueString'. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestHandleUniqueConstraintOk()
        {
            //SETUP
            using (var context = new Chapter10DbContext(_options))
            {
                context.Database.EnsureCreated();
                var checker = new SaveChangesWithSqlCheck( //#A
                    context, new Dictionary<int, FormatSqlException>
                {
                    [2601] = SqlErrorFormatters.UniqueErrorFormatter, //#B
                    [2627] = SqlErrorFormatters.UniqueErrorFormatter  //#B
                });
                var unique = Guid.NewGuid().ToString();

                //ATTEMPT
                context.Add(new MyUnique() { UniqueString = unique });
                var error = checker.SaveChangesWithChecking(); //#C
                /****************************************************************
                #A I create the SaveChangesWithSqlCheck class with its two parameters
                #B I provide a dictionary with a key of 2601 and 2627, violation of unique index, both paired with a method that can format that exception into a user-friendly format
                #C I call SaveChangesWithChecking, which returns null if there was no error, or a ValidationResult if there was a formatted error to show the user
                 * ****************************************************************/
                error.ShouldBeNull();
                context.Add(new MyUnique() { UniqueString = unique });
                var error2 = checker.SaveChangesWithChecking();

                //VERIFY
                error2.ShouldNotBeNull();
                error2.ErrorMessage.StartsWith("Cannot have a duplicate UniqueString in MyUnique. Duplicate value was ").ShouldBeTrue();
            }
        }

    }
}
