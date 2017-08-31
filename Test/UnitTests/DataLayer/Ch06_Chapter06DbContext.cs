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
                    context.Add(new MyEntityClass{ NormalProp = "Hello"});
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
        public void TestColumnNameInMemoryOk()
        {
            //SETUP
            using (var context = new Chapter06DbContext(
                EfInMemory.CreateNewContextOptions<Chapter06DbContext>()))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetColumnName(new MyEntityClass(), p => p.NormalProp);

                    //VERIFY
                    tableName.ShouldEqual("GenericDatabaseCol");
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
                    var tableName = context.GetColumnName(new MyEntityClass(), p => p.NormalProp);

                    //VERIFY
                    tableName.ShouldEqual("SqliteDatabaseCol");
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
                    var colType = context.GetColumnStoreType(new MyEntityClass(), p => p.NormalProp);

                    //VERIFY
                    colType.ShouldEqual("TEXT");
                }
            }
        }

        [Fact]
        public void TestGetColumnTypeSqlServerOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter06DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter06DbContext(optionsBuilder.Options))
            {
                //ATTEMPT
                var colType = context.GetColumnStoreType(new MyEntityClass(), p => p.NormalProp);

                //VERIFY
                colType.ShouldEqual("nvarchar(max)");
            }
        }


    }
}