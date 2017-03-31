// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch06_BackingFields
    {
        //private readonly ITestOutputHelper _output;

        //public Ch06_BackingFields(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestWriteEmptyPersonOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    context.Add(new Person());
                    context.SaveChanges();

                    //VERIFY
                }
            }
        }

        [Fact]
        public void TestMyProperySetGetOk()
        {
            //SETUP
            var options = EfInMemory.CreateNewContextOptions<Chapter06DbContext>();
            //ATTEMPT
            using (var context = new Chapter06DbContext(options))
            {
                var person = new Person {MyProperty = nameof(TestMyProperySetGetOk)};
                context.Add(person);
                context.SaveChanges();
            }
            //VERIFY
            using (var context = new Chapter06DbContext(options))
            {
                context.People.First().MyProperty.ShouldEqual(nameof(TestMyProperySetGetOk));
            }
        }

        [Fact]
        public void TestUpdatedOnSetGetOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter06DbContext>();
            optionsBuilder.UseSqlServer(connection);
            var now = DateTime.UtcNow;
            int personId;
            //ATTEMPT
            using (var context = new Chapter06DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                var person = new Person { UpdatedOn =  now};
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter06DbContext(optionsBuilder.Options))
            {
                context.People.Single(x => x.PersonId == personId).UpdatedOn.ShouldEqual(now);
                now.Kind.ShouldEqual(DateTimeKind.Utc);
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<DateTime>(x, "UpdatedOn"))
                    .Single().Kind.ShouldEqual(DateTimeKind.Unspecified);
            }
        }

        [Fact]
        public void TestDateOfBirthCalcOk()
        {
            //SETUP

            var tenYearsAgo = DateTime.Today.AddYears(-10).AddDays(-1);
            //ATTEMPT
            var person = new Person();
            person.SetDateOfBirth(tenYearsAgo);

            //VERIFY
            person.AgeYears.ShouldEqual(10);
        }

        [Fact]
        public void TestDateOfBirthSavedOk()
        {
            //SETUP
            var options = EfInMemory.CreateNewContextOptions<Chapter06DbContext>();
            var tenYearsAgo = DateTime.Today.AddYears(-10).AddDays(-1);
            int personId;
            //ATTEMPT
            using (var context = new Chapter06DbContext(options))
            {
                var person = new Person();
                person.SetDateOfBirth(tenYearsAgo);
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter06DbContext(options))
            {
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<DateTime>(x, "DateOfBirth"))
                    .Single().ShouldEqual(tenYearsAgo);
            }
        }

        //[Fact]
        //public void TestWriteEmptyPersonSqlServerOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter06DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter06DbContext(optionsBuilder.Options))
        //    {
        //        context.Database.EnsureCreated();

        //        //ATTEMPT
        //        context.Add(new Person());
        //        context.SaveChanges();

        //        //VERIFY
        //    }
        //}

    }
}