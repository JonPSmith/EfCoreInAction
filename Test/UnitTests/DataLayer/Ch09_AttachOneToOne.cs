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
    public class Ch09_AttachOneToOne
    {
        private readonly ITestOutputHelper _output;

        public Ch09_AttachOneToOne(ITestOutputHelper output)
        {
            _output = output;
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

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
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
    }
}
