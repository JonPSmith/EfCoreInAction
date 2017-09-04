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
    public class Ch09_UpdateCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch09_UpdateCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestChangeTrackerUpdateScalarOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.First();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateNewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntity();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity{ OneToOne = new OneEntity()};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities
                    .Include(x => x.OneToOne).First();
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
        public void TestChangeTrackerUpdateOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity { OneToOne = new OneEntity() };
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.OneEntities.First();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyEntityId,MyInt");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateOptionalWithNewRelationshipOk()
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
                var entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.Single();
                entity.OneToOne = new OneEntity();
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
                context.Add(new MyEntity());
                context.Add(new OneEntity());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.First();
                entity.OneToOne = context.OneEntities.First();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneToOne");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("MyEntityId");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateAsNoTrackingOk()
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

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.AsNoTracking().First();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateAsNoTrackingOptionalNotTrackedOneOk()
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

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.AsNoTracking().First();
                entity.OneToOne = context.OneEntities.AsNoTracking().First();
                context.Update(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneToOne");
                context.GetAllPropsNavsIsModified(entity.OneToOne).ShouldEqual("MyEntityId,MyInt");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateAsNoTrackingOptionalNewOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity());
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.AsNoTracking().First();
                entity.OneToOne = new OneEntity();
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
        public void TestChangeTrackerUpdateCollectionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.Single();
                var many = new ManyEntity();
                entity.Many.Add(many);
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
