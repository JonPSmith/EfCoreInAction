// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch06_Indexes
    {
        //private readonly ITestOutputHelper _output;

        //public Ch06_Chapter06DbContext(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestCreateIndexesOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    context.Add(entity);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(1);
                }
            }
        }

        [Fact]
        public void TestCreateIndexesNullOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    context.Add(entity);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(1);
                }
            }
        }

        [Fact]
        public void TestCreateMulipleIndexesOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "b"
                    };
                    context.AddRange(entity1,entity2);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(2);
                }
            }
        }

        [Fact]
        public void TestCreateMulipleIndexesBad()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    context.AddRange(entity1, entity2);
                    var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                    //VERIFY
                    ex.InnerException.Message.ShouldEqual(
                        "SQLite Error 19: 'UNIQUE constraint failed: IndexClasses.IndexUnique'");
                }
            }
        }


        [Fact]
        public void TestCreateMulipleIndexesNullOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    context.AddRange(entity1, entity2);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(2);
                }
            }
        }

        [Fact]
        public void TestCreateMulipleIndexesSqlBad()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter06DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter06DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity1 = new IndexClass
                {
                    IndexNonUnique = "a",
                    IndexUnique = "a"
                };
                var entity2 = new IndexClass
                {
                    IndexNonUnique = "a",
                    IndexUnique = "a"
                };
                context.AddRange(entity1, entity2);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    @"Cannot insert duplicate key row in object 'dbo.IndexClasses' with unique index 'MyUniqueIndex'. The duplicate key value is (a).
The statement has been terminated.");
            }
        }


    }
}