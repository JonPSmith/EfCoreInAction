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
    public class Ch09_AddAlterOneToOne
    {
        private readonly ITestOutputHelper _output;

        public Ch09_AddAlterOneToOne(ITestOutputHelper output)
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
                var track = new MyEntity {OneToOne = new OneEntity()};
                context.Add(track);
                var notify = new NotifyEntity {OneToOne = new NotifyOne()};
                context.Add(notify);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
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
                context.Add(new MyEntity ());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.OneToOne = new OneEntity();
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Include(x => x.OneToOne).Single().ShouldNotBeNull();
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
                entity.OneToOne = new NotifyOne();
                context.SaveChanges();

                //VERIFY
                context.Notify.Include(x => x.OneToOne).Single().ShouldNotBeNull();
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
                var entity = new MyEntity {OneToOne = new OneEntity()};
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerAddTrackedOneTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new OneEntity());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var one = context.OneEntities.Single();
                var entity = new MyEntity { OneToOne = one };
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOne");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("MyEntityId");
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
                entity.OneToOne = new NotifyOne();
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
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
                context.Add(new MyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOne).Single();
                entity.OneToOne = new OneEntity();

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
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
                entity.OneToOne = new NotifyOne();

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new MyEntity();
                track.OneToOne = new OneEntity();
                context.Add(track);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOne).Single();
                var oldOneToOne = entity.OneToOne;
                entity.OneToOne = new OneEntity();

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(3);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetEntityState(oldOneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(oldOneToOne).ShouldEqual("MyEntityId");
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceTrackedAfterSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new MyEntity();
                track.OneToOne = new OneEntity();
                context.Add(track);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOne).Single();
                var oldOneToOne = entity.OneToOne;
                entity.OneToOne = new OneEntity();
                context.SaveChanges();

                //VERIFY
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(oldOneToOne).ShouldEqual(EntityState.Unchanged);
                context.OneEntities.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceExistingTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity());
                context.Add(new OneEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOne).Single();
                var existing = context.OneEntities.First();
                entity.OneToOne = existing;

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(existing).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOne");
                context.GetAllPropsNavsIsModified(existing).ShouldEqual("MyEntityId");
            }
        }
    }
}
