using System.Data;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using test.Attributes;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_AccessEfServices
    {
        [RunnableInDebugOnly]
        public void GetServiceScopeProvider()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT 
                var serviceProvider = context.GetService<IServiceScopeFactory>();

                //VERIFY

            }
        }

        [Fact]
        public void GetIModelValidator()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT 
                var service = context.GetService<IModelValidator>();

                //VERIFY
                service.ShouldNotBeNull();
                service.ShouldBeType<SqliteModelValidator>();
            }
        }

        [Fact]
        public void UseIModelValidatorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                var service = context.GetService<IModelValidator>();

                //ATTEMPT 
                service.Validate(context.Model);

                //VERIFY
            }
        }

        [Fact]
        public void UseITypeMapperSqlToNetSqlServerOk()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection(); //#A
            using (var context = new EfCoreContext(optionsBuilder.Options)) //#B
            {
                var service = context.GetService<IRelationalTypeMapper>(); //#C

                //ATTEMPT 
                var netTypeInfo = service.FindMapping("varchar(20)"); //#D

                //VERIFY
                netTypeInfo.ClrType.ShouldEqual(typeof(string));//#E
                netTypeInfo.IsUnicode.ShouldBeFalse();          //#F
                netTypeInfo.Size.ShouldEqual(20);               //#F
            }
            /*********************************************************************
            #A The mapping depends on the the database we are connecting to. In this case I am using a SQL Server
            #B I have to create an instance of the application's DbContext to access the services
            #C I use the GetService<T> method to get the IRelationalMapper - this will be mapped to the database provider's mapper
            #D Now I can use this service find the mapping from a SQL type to a .NET type
            #E These are unit test checks that verify that the .NET version would be a string
            #F These unit test checks confirm the EF Core configuration parts that would be needed to property map a string to the specific SQL type
             * ******************************************************************/
        }

        [Fact]
        public void UseITypeMapperNetToSqlSqlServerOk()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection();
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var service = context.GetService<IRelationalTypeMapper>();

                //ATTEMPT 
                IProperty property = context.GetProperty(new Book(), book => book.ImageUrl);
                var sqlTypeInfo = service.FindMapping(property);

                //VERIFY
                sqlTypeInfo.DbType.ShouldEqual(DbType.AnsiString);
                sqlTypeInfo.StoreType.ShouldEqual("varchar(512)");
                sqlTypeInfo.Size.ShouldEqual(512);
            }
        }



        [Fact]
        public void CheckKeyPropagatorOk()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection();
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {

                //ATTEMPT 
                var service = context.GetService<IKeyPropagator>();

                //VERIFY
                service.ShouldNotBeNull();
                service.ShouldBeType<KeyPropagator>();
            }
        }
    }
}
