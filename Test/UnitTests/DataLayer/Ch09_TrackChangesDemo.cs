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
    public class Ch09_TrackChangesDemo
    {
        private readonly ITestOutputHelper _output;

        public Ch09_TrackChangesDemo(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestEntityScalarStagesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT && Verify
                var entity = new MyEntity();
                entity.MyString = "Test";
                context.GetEntityState(entity).ShouldEqual(EntityState.Detached);
                context.Add(entity);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.SaveChanges();
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                entity.MyString = "New String";
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.SaveChanges();
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.Remove(entity);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.SaveChanges();
                context.GetEntityState(entity).ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public void TestSaveChangesDoNotAcceptChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT && Verify
                var entity = new MyEntity();
                entity.MyString = "Test";
                context.Add(entity);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.SaveChanges(false);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);

                context.MyEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSaveChangesDoNotAcceptChangesCallAgainOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT && Verify
                var entity = new MyEntity();
                entity.MyString = "Test";
                context.Add(entity);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.SaveChanges(false);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.SaveChanges();

                context.MyEntities.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestModifyNewOneToOneStagesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new MyEntity();
                entity.MyString = "Test";
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = 
                    context.MyEntities.First();
                var oneToOne = new OneEntity();
                entity.OneToOne = oneToOne;

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(oneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(oneToOne).ShouldEqual("");
            }
        }


    }
}
