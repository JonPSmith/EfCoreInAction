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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public class CustomSqlServerTypeMapper //#A
            : SqlServerTypeMapper              //#A
        {
            public CustomSqlServerTypeMapper(                 //#B
                RelationalTypeMapperDependencies dependencies)//#B 
                : base(dependencies) {}                       //#B

            public override RelationalTypeMapping 
                FindMapping(IProperty property) //#C
            {
                var currentMapping = base.FindMapping(property); //#D
                if (property.ClrType == typeof(string)   //#E
                    && property.Name.EndsWith("Ascii"))  //#E
                {
                    var size = currentMapping.Size == null //#F
                        ? "max"                            //#F
                        : currentMapping.Size.ToString();  //#F
                    return new StringTypeMapping(//#G
                        $"varchar({size})",      //#G
                        DbType.AnsiString, true, //#G
                        currentMapping.Size);    //#G
                }

                return currentMapping; //#I
            }
        }
        /***********************************************************
        #A I create my custom type mapper by inheriting the SqlSever type mapper. 
        #B I need to add a constructor that passes the dependencies it needs to the inherited class
        #C I only override the FindMapping method that deals with .NET type to SQL type. All the other mapping methods I leave as they were
        #D I get the mapping that the Sql Server database provider would nomally do. This gives me information I can use
        #E This is where I insert my new rule. If the property is of .NET type 'string' and the property name ends with "Ascii" then I want to set it as a SQL varchar, instead of the normal SQL nvarchar
        #F I work out the size part of SQL type string. Either the size provided, or "max" if the size is null
        #G This builds a StringTypeMapping with the various parts set to a 'varchar' type column, that is, an 8-bit character string
        #H If the property didn’t fit my new rule, then I want the normal EF Core mapping. I therefore I return the SQL type mapping the base method has calculated
         * **********************************************************/

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


        private void DummyMethodWithAspNetCoreStartup(IServiceCollection services)
        {
            IConfiguration Configuration = null;

            var connection = Configuration
                .GetConnectionString("DefaultConnection");
            services.AddDbContext<EfCoreContext>(           //#A
                options => options.UseSqlServer(connection, //#A
                    b => b.MigrationsAssembly("DataLayer")) //#A
            .ReplaceService<IRelationalTypeMapper, CustomSqlServerTypeMapper>() //#B
                    );
            /********************************************************
            #A This is the normal code register the EFCoreContext class, which is my application's DbContext, and its options with ASP.NET Core dependency injection module
            #B This is the new code that replaces the normal relational type mapper with my modified type mapper
             * ****************************************************/
        }
    }
}
