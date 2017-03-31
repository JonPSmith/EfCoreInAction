// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using test.EfHelpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch06_Chapter06DbContext
    {
        //private readonly ITestOutputHelper _output;

        //public Ch06_Chapter06DbContext(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void WriteToDatabaseOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    context.Add(new MyEntityClass{ InDatabaseProp = "Hello"});
                    context.SaveChanges();

                    //VERIFY
                }
            }
        }

        [Fact]
        public void TestTableNameOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetTableName<MyEntityClass>();

                    //VERIFY
                    tableName.ShouldEqual("MyTable");
                }
            }
        }

        [Fact]
        public void TestColumnNameRelationalOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetColumnName(new MyEntityClass(), p => p.InDatabaseProp);

                    //VERIFY
                    tableName.ShouldEqual("GenericInDatabaseProp");
                }
            }
        }

        [Fact]
        public void TestColumnNameSqliteOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetColumnNameSqlite(new MyEntityClass(), p => p.InDatabaseProp);

                    //VERIFY
                    tableName.ShouldEqual("SqliteInDatabaseProp");
                }
            }
        }

        [Fact]
        public void TestColumnTypeSqliteOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT
                    var colType = context.GetColumnStoreType(new MyEntityClass(), p => p.InDatabaseProp);

                    //VERIFY
                    colType.ShouldEqual("TEXT");
                }
            }
        }

        //[Fact]
        //public void TestGetColumnRelationalTypeSqlDatabase()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter06DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter06DbContext(optionsBuilder.Options))
        //    {
        //        //ATTEMPT
        //        var colType = context.GetColumnRelationalType(new MyEntityClass(), p => p.InDatabaseProp);

        //        //VERIFY
        //        colType.ShouldEqual("nvarchar(max)");
        //    }
        //}


    }
}