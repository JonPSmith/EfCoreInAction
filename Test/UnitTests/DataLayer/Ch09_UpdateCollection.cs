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
    public class Ch09_UpdateCollection
    {
        private readonly ITestOutputHelper _output;

        public Ch09_UpdateCollection(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var track = new TrackedEntity();
                track.Collection.Add(new SubTrackedEntity());
                context.Add(track);
                var notify = new NotifyEntity();
                notify.Collection.Add(new SubNotifyEntity());
                context.Add(notify);
                context.SaveChanges();

                //VERIFY
                context.Tracked.Count().ShouldEqual(1);
                context.Notify.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestUpdateTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TrackedEntity ());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Tracked.Single();
                entity.Collection.Add(new SubTrackedEntity());
                context.SaveChanges();

                //VERIFY
                context.Tracked.Single().Collection.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestUpdateNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new NotifyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.Collection.Add(new SubNotifyEntity());
                context.SaveChanges();

                //VERIFY
                context.Notify.Single().Collection.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangeTrackerAddTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new TrackedEntity();
                entity.Collection.Add(new SubTrackedEntity());
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetPropertyIsModified(entity, x => x.MyString).ShouldBeFalse();
            }
        }

        [Fact]
        public void TestChangeTrackerAddNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new NotifyEntity();
                entity.Collection.Add(new SubNotifyEntity());
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetPropertyIsModified(entity, x => x.MyString).ShouldBeFalse();
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TrackedEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Tracked.Include(x => x.Collection).Single();
                entity.Collection.Add(new SubTrackedEntity());

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetNavigationalIsModified(entity, x => x.Collection).ShouldBeFalse();
                context.GetEntityState(entity.Collection.First()).ShouldEqual(EntityState.Added);
           }
        }

        [Fact]
        public void TestChangeTrackerUpdateNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new NotifyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.Collection.Add(new SubNotifyEntity());

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetNavigationalIsModified(entity, x => x.Collection).ShouldBeFalse();
                context.GetEntityState(entity.Collection.First()).ShouldEqual(EntityState.Added);
            }
        }
    }
}
