// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_NotiyNotify2
    {
        private readonly ITestOutputHelper _output;

        public Ch09_NotiyNotify2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestNotifyModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new NotifyEntity {MyString = "Test"};
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity =
                    context.Notify.First();
                entity.MyString = "Changed";

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
            }
        }

        [Fact]
        public void TestNotify2ModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new Notify2Entity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity =
                    context.Notify2.First();
                entity.MyString = "Changed";

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
            }
        }


        [Fact]
        public void TestNotifyHasOriginalValuesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new NotifyEntity() { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity =
                    context.Notify.First();
                entity.MyString = "Changed";

                //VERIFY
                context.Entry(entity).Property(nameof(NotifyEntity.MyString)).OriginalValue.ShouldEqual("Test");
            }
        }

        [Fact]
        public void TestNotify2HasNoOriginalValuesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new Notify2Entity() { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity =
                    context.Notify2.First();
                entity.MyString = "Changed";
                var ex = Assert.Throws<InvalidOperationException>(() => context.Entry(entity)
                    .Property(nameof(NotifyEntity.MyString)).OriginalValue);

                //VERIFY
                ex.Message.StartsWith("The original value for property 'MyString' of entity type 'Notify2Entity' cannot be accessed because it is not being tracked. Original values are not recorded for most properties of entities when the 'ChangingAndChangedNotifications' strategy is used. ")
                    .ShouldBeTrue();
            }
        }
    }
}
