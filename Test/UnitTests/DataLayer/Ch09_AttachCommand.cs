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
    public class Ch09_AttachCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch09_AttachCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAttachNoRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                context.Attach(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangeTrackerAttachNoRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestAttachOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                entity.OneToOne = new OneEntity();
                context.Attach(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.MyEntities.Include(x => x.OneToOne).Single(x => x.Id == entity.Id).OneToOne.ShouldNotBeNull();
                context.OneEntities.Count().ShouldEqual(1);

            }
        }

        [Fact]
        public void TestAttachRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            NotifyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new NotifyEntity ();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter09DbContext(options))
            {
                entity.OneToOne = new NotifyOne();
                context.Attach(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //VERIFY
                context.Notify.Count().ShouldEqual(1);
                context.Set<NotifyOne>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangeTrackerAttachOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                entity.OneToOne = new OneEntity();
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerAttachRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            NotifyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new NotifyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                entity.OneToOne = new NotifyOne();
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestAttachOptionalNoRelationshipTrackedOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.Add(new OneEntity());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var oneToOne = context.OneEntities.First();
                entity.OneToOne = oneToOne;
                context.Attach(entity);
                context.SaveChanges();
                //Nothing happens, because the state is unchanged
            }
            using (var context = new Chapter09DbContext(options))
            {
                //VERIFY
                context.OneEntities.First().MyEntityId.ShouldBeNull();
                context.MyEntities.Include(p => p.OneToOne).First().OneToOne.ShouldBeNull();
            }
        }

        [Fact]
        public void TestChangeTrackerAttachOptionalNoRelationshipTrackedOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity();
                context.Add(entity);
                context.Add(new OneEntity());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var oneToOne = context.OneEntities.First();
                entity.OneToOne = oneToOne;
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }

        [Fact]
        public void TestAttachOptionalwithRelationshipTrackedOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity {OneToOne = new OneEntity()};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var oneToOne = context.OneEntities.First();
                entity.OneToOne = oneToOne;
                context.Attach(entity);
                context.SaveChanges();
                //Nothing happens, because the state is unchanged
            }
            using (var context = new Chapter09DbContext(options))
            {
                //VERIFY
                context.OneEntities.First().MyEntityId.ShouldNotBeNull();
                context.MyEntities.Include(p => p.OneToOne).First().OneToOne.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestChangeTrackerAttachOptionalWithRelationshipTrackedOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            MyEntity entity;
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                entity = new MyEntity { OneToOne = new OneEntity() };
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var oneToOne = context.OneEntities.First();
                entity.OneToOne = oneToOne;
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("");
            }
        }
    }
}
