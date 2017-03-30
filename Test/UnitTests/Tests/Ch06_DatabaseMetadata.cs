// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch06_DatabaseMetadata
    {
        [Fact]
        public void TestGetTableNameOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var tableName = context.GetTableName<Book>();

                //VERIFY
                tableName.ShouldEqual("Books");
            }
        }

        [Fact]
        public void TestGetColumnNameOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var colName = context.GetColumnName(new Book(), p => p.BookId);

                //VERIFY
                colName.ShouldEqual("BookId");
            }
        }

        //see https://github.com/aspnet/EntityFramework/issues/8034
        [Fact]
        public void TestGetColumnRelationalTypeOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var colType = context.GetColumnRelationalType(new Book(), p => p.Description);

                //VERIFY
                colType.ShouldEqual("nvarchar(max)");
            }
        }

        [Fact]
        public void TestGetColumnSqliteTypeTypeOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var colType = context.GetColumnSqliteType(new Book(), p => p.PublishedOn);

                //VERIFY
                colType.ShouldEqual("date");
            }
        }

        //see https://github.com/aspnet/EntityFramework/issues/8034
        [Fact]
        public void TestGetColumnRelationalTypeSQlDatabase()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var colType = context.GetColumnRelationalType(new Book(), p => p.Description);

                //VERIFY
                colType.ShouldEqual("nvarchar(max)");
            }
        }

    }
}