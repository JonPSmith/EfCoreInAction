// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
    public class Ch09_UpdateOneToOne
    {
        private readonly ITestOutputHelper _output;

        public Ch09_UpdateOneToOne(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestChangeTrackerUpdateOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new TrackedEntity{ OneToOne = new TrackedOne()};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.Tracked.Include(x => x.OneToOne).Single();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateOptionalWithNewRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            TrackedEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new TrackedEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                entity.OneToOne = new TrackedOne();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateTrackedOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new TrackedEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.Tracked.Single();
                entity.OneToOne = new TrackedOne();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateTrackedOptionalTrackedOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TrackedEntity());
                context.Add(new TrackedOne());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.Tracked.Single();
                entity.OneToOne = context.Set<TrackedOne>().Single();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneToOne");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("TrackedEntityId");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateCollectionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new TrackedEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.Tracked.Single();
                var many = new TrackedMany();
                entity.Collection.Add(many);
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(many).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.GetAllPropsNavsIsModified(many).ShouldEqual("");
            }
        }
    }
}
