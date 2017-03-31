// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using test.EfHelpers;
using Test.Chapter06Listings;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch06_DatabaseMetadata
    {
        private readonly ITestOutputHelper _output;

        public Ch06_DatabaseMetadata(ITestOutputHelper output)
        {
            _output = output;
        }

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

        [Theory]
        [InlineData("MyEntityClassId")]
        [InlineData("InDatabaseProp")]
        public void ListColumnName(string propName)
        {
            using (var context = new Chapter06DbContext(
                SqliteInMemory.CreateOptions<Chapter06DbContext>()))
            {
                _output.WriteLine("Property {0}:\t column name = {1}", propName, context.GetColumnName<MyEntityClass>(propName));
            }
        }

        [Fact]
        public void TestGetColumnStoreTypeOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var colType = context.GetColumnStoreType(new Book(), p => p.Description);

                //VERIFY
                colType.ShouldEqual("TEXT");
            }
        }

        [Fact]
        public void TestGetColumnStoreTypesDirect()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            //ATTEMPT
            using (var context = inMemDb.GetContextWithSetup())
            {
                var efProperty = context.Entry(new Order()).Metadata.GetProperties().Single(x => x.Name == "DateOrderedUtc");
                var typeMapper = context.GetService<IRelationalTypeMapper>();
                var mappings = typeMapper.FindMapping(efProperty);
                mappings.StoreType.ShouldEqual("TEXT");

            }
        }

        [Theory]
        [InlineData("BookId", "int")]
        [InlineData("PublishedOn", "date")]
        [InlineData("Price", "decimal(9,2)")]
        [InlineData("Description", "nvarchar(max)")]
        [InlineData("ImageUrl", "varchar(512)")]
        public void TestGetColumnStoreTypesOk(string propName, string expectedType)
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                var colType = context.GetColumnStoreType<Book>(propName);
                colType.ShouldEqual(expectedType);
            }
        }

    }
}