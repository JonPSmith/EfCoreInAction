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
    public class Ch09_NonDbGeneratedKey
    {
        private readonly ITestOutputHelper _output;

        public Ch09_NonDbGeneratedKey(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid();
                context.Add(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestAddWithNewOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid {OneToOne = new OneEntityGuid()};
                context.Add(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
                
            }
        }

        [Fact]
        public void TestAddWithExistingOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new OneEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = context.OneEntityGuids.First() };
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
            }
        }

        //-----------------------------------------

        [Fact]
        public void TestRemoveOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntityGuids.First();
                context.Remove(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
            }
        }

        [Fact]
        public void TestRemoveWithNewOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntityGuids.First();
                entity.OneToOne = new OneEntityGuid();
                context.Remove(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestRemoveWithNewOneEntityKeySetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntityGuids.First();
                entity.OneToOne = new OneEntityGuid{ Id = Guid.NewGuid()};
                context.Remove(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestRemoveWithExistingOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new MyEntityGuid());
                context.Add(new OneEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntityGuids.First();
                entity.OneToOne = context.OneEntityGuids.First();

                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
            }
        }

        //-----------------------------------------

        [Fact]
        public void TestUpdateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid {Id = Guid.NewGuid()};
                context.Update(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestUpdateWithNewOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = new OneEntityGuid() };
                context.Update(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestUpdateWithNewOneEntityKetSetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = new OneEntityGuid{ Id = Guid.NewGuid()} };
                context.Update(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestUpdateWithExistingOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new OneEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = context.OneEntityGuids.First() };
                context.Update(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
            }
        }

        //-----------------------------------------

        [Fact]
        public void TestAttachKeyDefaultOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid();
                context.Attach(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestAttachKeySetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid { Id = Guid.NewGuid() };
                context.Attach(entity);

                //VERIFY
                entity.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestAttachWithNewOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = new OneEntityGuid() };
                context.Attach(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestAttachWithNewOneEntityKeySetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = new OneEntityGuid{ Id = Guid.NewGuid()} };
                context.Attach(entity);

                //VERIFY
                entity.OneToOne.Id.ShouldNotEqual(Guid.Empty);
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestAttachWithExistingOneEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();
            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new OneEntityGuid());
                context.SaveChanges();
            }
            using (var context = new Chapter09DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntityGuid { OneToOne = context.OneEntityGuids.First() };
                context.Attach(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOne).ShouldEqual(EntityState.Modified);
            }
        }
    }
}
