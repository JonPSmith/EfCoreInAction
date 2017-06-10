// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EFCode;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_ModelProperty
    {
        private readonly ITestOutputHelper _output;

        public Ch09_ModelProperty(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GetTableNameOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var eType = context.Model
                    .FindEntityType(typeof(Book).FullName);
                var bookTableName = eType
                    .Relational().TableName;

                //VERIFY
                bookTableName.ShouldEqual("Books");
            }
        }

        [Fact]
        public void OutputAllRelationshipsEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                ListToEntitiesWithRelationships(context);
            }
        }

        private void ListToEntitiesWithRelationships(DbContext context)
        {
            var allEntities = context.Model.GetEntityTypes().ToList();
            foreach (var entity in allEntities)
            {
                _output.WriteLine($"{entity}");
                var fKeys = entity.GetForeignKeys().ToList();
                if (fKeys.Any())
                {
                    _output.WriteLine("      Principals are:");
                    foreach (var fKey in fKeys)
                    {
                        _output.WriteLine($"           {fKey.PrincipalEntityType}");
                    }
                }

            }
        }

        [Fact]
        public void GetTableNamesInOrderToDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var tableNames = context.GetTableNamesInOrderForDelete();

                //VERIFY
                tableNames
                    .ShouldEqual( new []{ "BookAuthor", "LineItem", "PriceOffers", "Review", "Authors", "Books", "Orders" });

            }
        }

        [Fact]
        public void GetTableNamesInOrderToDeleteErrorOnHierarchicalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.GetTableNamesInOrderForDelete());

                //VERIFY
                ex.Message.ShouldEqual("You cannot delete all the EntityType: Employee rows in one go.");
            }
        }


        [Fact]
        public void ClearAllTablesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                foreach (var tableName in 
                    context.GetTableNamesInOrderForDelete())
                {
                    context.Database
                        .ExecuteSqlCommand(
                            $"DELETE FROM {tableName}");
                }
            }
            using (var context = new EfCoreContext(options))
            {
                //VERIFY
                context.Books.Count().ShouldEqual(0);
                context.Authors.Count().ShouldEqual(0);
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void OutputAllRelationshipsWipeDbContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<WipeDbContext>();
            using (var context = new WipeDbContext(options))
            {
                ListToEntitiesWithRelationships(context);
            }
        }

        [Fact]
        public void GetTableNamesInOrderToDeleteWipeDbContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<WipeDbContext>();
            using (var context = new WipeDbContext(options))
            {
                //ATTEMPT
                var tableNames = context.GetTableNamesInOrderForDelete();

                //VERIFY
                tableNames
                    .ShouldEqual(new[] { "T1P1", "T2P4", "T1P2", "T1P3", "T1P4", "T2P3", "T2P2", "T2P1"});

            }
        }

        [Fact]
        public void ClearAllTablesChapter09DbContextWipeCheckOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<WipeDbContext>();
            using (var context = new WipeDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new T1P1 { T1P2 = new T1P2 { T1P3 = new T1P3 { T1P4 = new T1P4()}}});
                context.Add(new T2P1 {T2P2 = new T2P2{ T2P3 = new T2P3{ T2P4 = new T2P4()}}});
                context.SaveChanges();

                //ATTEMPT
                foreach (var tableName in
                    context.GetTableNamesInOrderForDelete())
                {
                    context.Database
                        .ExecuteSqlCommand(
                            $"DELETE FROM {tableName}");
                }
            }
            using (var context = new WipeDbContext(options))
            {
                //VERIFY
                context.T2P1.Count().ShouldEqual(0);
            }
        }
    }
}
