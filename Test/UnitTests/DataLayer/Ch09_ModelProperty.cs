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
using Test.Chapter09Listings.WipeDbClasses;
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
        public void GetTableNameEfCoreContextOk()
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
                var tableNames = string.Join(",", context.GetTableNamesInOrderForWipe());

                //VERIFY
                tableNames.ShouldEqual("BookAuthor,LineItem,PriceOffers,Review,Orders,Books,Authors");

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
                var ex = Assert.Throws<InvalidOperationException>(() => context.GetTableNamesInOrderForWipe());

                //VERIFY
                ex.Message.ShouldEqual("You cannot delete all the rows in one go in entity(s): Test.Chapter07Listings.EfClasses.Employee");
            }
        }


        [Fact]
        public void WipeAllTablesEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.WipeAllDataFromDatabase();
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
                var tableNames = string.Join(",", context.GetTableNamesInOrderForWipe());

                //VERIFY
                tableNames.ShouldEqual("T2P4,T2P3,T2P2,T2P1,Top,T1P1,T1P2,T1P3,T1P4,SelfRef");
            }
        }

        [Fact]
        public void WipeAllTablesChapter09DbContextWipeCheckOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<WipeDbContext>();
            using (var context = new WipeDbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add( new TopEntity
                {
                    T1P1 = new T1P1 { T1P2 = new T1P2 { T1P3 = new T1P3 { T1P4 = new T1P4() } } },
                    T2P1 = new T2P1 { T2P2 = new T2P2 { T2P3 = new T2P3 { T2P4 = new T2P4() } } }
                });
                context.Add(new SelfRef{ Name ="Staff", Manager = new SelfRef{ Name = "Manager"}});
                context.SaveChanges();

                //ATTEMPT
                context.WipeAllDataFromDatabase();
            }
            using (var context = new WipeDbContext(options))
            {
                //VERIFY
                context.Top.Count().ShouldEqual(0);
            }
        }
    }
}
