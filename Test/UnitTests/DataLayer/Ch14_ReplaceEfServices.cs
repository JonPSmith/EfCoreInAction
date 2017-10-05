using System;
using System.Data;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter14Listings.EfClasses;
using Test.Chapter14Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_ReplaceEfServices
    {
        public class CustomSqlServerTypeMapper : SqlServerTypeMapper
        {
            public CustomSqlServerTypeMapper(RelationalTypeMapperDependencies dependencies) 
                : base(dependencies) {}

            public override RelationalTypeMapping FindMapping(IProperty property)
            {
                var currentMapping = base.FindMapping(property);
                if (property.ClrType == typeof(string) && property.Name.EndsWith("Ascii"))
                    return new StringTypeMapping($"varchar({currentMapping.Size?.ToString() ?? "max"})", 
                       DbType.AnsiString, true, currentMapping.Size);

                return currentMapping;
            }

            public override RelationalTypeMapping FindMapping(string storeType)
            {
                return base.FindMapping(storeType);
            }
        }

        public class CustomSqlServerModelValidator : SqlServerModelValidator
        {
            public CustomSqlServerModelValidator(ModelValidatorDependencies dependencies, 
                RelationalModelValidatorDependencies relationalDependencies) : base(dependencies, relationalDependencies)
            {
            }

            public override void Validate(IModel model)
            {
                var classesWhereTableDoesNotEndWithS =
                    model.GetEntityTypes().Where(x => !x.Relational().TableName.EndsWith("s")).ToList();
                if (classesWhereTableDoesNotEndWithS.Any())
                    throw new InvalidOperationException(
                        "The custom validator has found the following entities with a table name that does not end in s: " +
                        string.Join(", ", classesWhereTableDoesNotEndWithS.Select(x => x.ClrType.Name)));

                base.Validate(model);
            }
        }

        [Fact]
        public void UseITypeMapperOk()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection();
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var service = context.GetService<IRelationalTypeMapper>();

                //ATTEMPT 
                var mapInfo = service.FindMapping("varchar(20)");

                //VERIFY
                mapInfo.ClrType.ShouldEqual(typeof(string));
                mapInfo.IsUnicode.ShouldBeFalse();
            }
        }

        [Fact]
        public void ReplaceTypeMapperOk()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<MappingDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.ReplaceService<IRelationalTypeMapper, CustomSqlServerTypeMapper>();

            using (var context = new MappingDbContext(optionsBuilder.Options))
            {
                //ATTEMPT 
                var entity = context.Model.FindEntityType(typeof(ScalarEntity));
                var mapInfo1 = entity.GetProperty(nameof(ScalarEntity.StringAscii));
                var sType = context.GetColumnStoreType(new ScalarEntity(), p => p.StringAscii);

                //VERIFY
                mapInfo1.ClrType.ShouldEqual(typeof(string));
                sType.ShouldEqual("varchar(max)");
            }
        }

        [Fact]
        public void ReplaceModelValidatorOk()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.ReplaceService<IModelValidator, CustomSqlServerModelValidator>();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                //ATTEMPT 
                var ex = Assert.Throws<InvalidOperationException>(() => context.Database.EnsureCreated());

                //VERIFY
                ex.Message.EndsWith("BookAuthor, LineItem, Review").ShouldBeTrue();
            }
        }
    }
}
