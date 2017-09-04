// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_TrackGraph
    {
        private readonly ITestOutputHelper _output;

        public Ch09_TrackGraph(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSettingIsModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {

                //ATTEMPT
                var entity = new MyEntity();
                context.Entry(entity).Property("MyString").IsModified = true;

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestClearingIsModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {

                //ATTEMPT
                var entity = new MyEntity();
                entity.MyString = "New";
                context.Entry(entity).Property(nameof(MyEntity.MyString)).IsModified = false;

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(0);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
                context.GetEntityState(entity).ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public void TestTrackGraphDisconnectedAuthorUpdateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            string json;
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var book = context.Books                      
                    .Where(p => p.Title == "Quantum Networking")
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single();
                book.AuthorsLink.First().Author.Name = "New Person";
                json = JsonConvert.SerializeObject(book, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Formatting = Formatting.Indented
                });
            }
            using (var context = new EfCoreContext(options))
            {
                var book = JsonConvert
                    .DeserializeObject<Book>(json, new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });

                //ATTEMPT
                //var book = untracked book with all relationships //#A
                context.ChangeTracker.TrackGraph(book, e => //#B
                {
                    e.Entry.State = EntityState.Unchanged; //#C
                    if (e.Entry.Entity is Author) //#D
                    {
                        e.Entry.Property("Name").IsModified = true; //#E
                    }
                });
                //context.SaveChanges(); //#F
                /**********************************************
                #A I expect an untracked book with its relationships
                #B I call ChangeTracker.TrackGraph, which takes an entity instance and an Func method. The Func method is called once on each entity in the graph of entities.
                #C If the method sets the state to any value other than Disconnected the the entity will become tracked by EF Core
                #D In this example I only want to set the Name property of the Author entity as modified, so I check if the entity is of type Author
                #E I set the IsModified flag on the Name property. This will also set the State of the entity to Modified
                #F I finally call SaveChanges, which finds that only the Name property of the Author entity has been marked as changed, and therefore creates the optimal SQL to update the Name column in the Authors table
                * *********************************************/

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(6);
                foreach (var entity in context.ChangeTracker.Entries())
                {
                    _output.WriteLine("{0}: State = {1}", entity.Metadata.Name, entity.State);
                }
                context.ChangeTracker.Entries().Where(x => x.Metadata.Name != typeof(Author).FullName)
                    .All(x => x.State == EntityState.Unchanged).ShouldBeTrue();
                context.GetEntityState(book.AuthorsLink.First().Author).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(book.AuthorsLink.First().Author).ShouldEqual("Name");
            }
        }


    }
}
