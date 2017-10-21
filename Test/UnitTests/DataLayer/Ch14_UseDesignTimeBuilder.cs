using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter14Listings.EfServices;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_UseDesignTimeBuilder
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionString;
        private readonly DbContextOptions<EfCoreContext> options;

        public Ch14_UseDesignTimeBuilder(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = this.GetUniqueDatabaseConnectionString();
            options = this.ClassUniqueDatabaseSeeded4Books();
        }

        [Fact]
        public void TestSqlServerProviderName()
        {
            //SETUP

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var providerName = context.Database.ProviderName;

                //VERIFY
                providerName.ShouldEqual(DesignTimeBuilder.SqlServerProviderName);
            }
        }

        [Fact]
        public void TestMySqlProviderName()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration().GetConnectionString("MySqlDatabaseUnitTest");
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection);

            //ATTEMPT
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var providerName = context.Database.ProviderName;

                //VERIFY
                providerName.ShouldEqual(DesignTimeBuilder.MySqlProviderName);
            }
        }


        [Fact]
        public void TestGetDatabaseModel()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IDatabaseModelFactory>();

            //ATTEMPT 
            var model = factory.Create(_connectionString, new string[] { }, new string[] { });

            //VERIFY
            model.ShouldNotBeNull();
        }

        [Fact]
        public void TestDatabaseModelTableNames()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IDatabaseModelFactory>();

            var model = factory.Create(_connectionString, new string[] { }, new string[] { });

            //ATTEMPT 
            var tables = model?.Tables;

            //VERIFY
            tables.Select(x => x.Name).ShouldEqual(new[] { "Authors", "Books", "Orders", "BookAuthor", "PriceOffers", "Review", "LineItem"});
        }

        [Fact]
        public void ListDatabaseModel()
        {
            var builder = new DesignTimeBuilder();  //#A
            var serviceProvider = builder                          //#B
                .GetScaffolderService(DatabaseProviders.SqlServer);//#B
            var service = serviceProvider             //#C
                .GetService<IDatabaseModelFactory>(); //#C

            var model = service.Create(_connectionString,//#D 
                new string[] { }, new string[] { });     //#E

            foreach (var table in model.Tables) //#F
            {
                _output.WriteLine(                                    //#G
                    $"CREATE TABLE [{table.Schema}].[{table.Name}](");//#G
                foreach (var column in table.Columns) //#H
                {
                    var pKey = table.PrimaryKey.Columns.Contains(column)//#I
                        ? (column.ValueGenerated == ValueGenerated.OnAdd//#I 
                           ? " IDENTITY(1,1)" : "")                     //#I
                        : "";                                           //#I
                    var nullable = (column.IsNullable //#J
                        ? "" : "NOT ") + "NULL";      //#J
                    _output.WriteLine($"   [{column.Name}] "+      //#K
                        $"[{column.StoreType}]{pKey} {nullable},");//#K
                }
                _output.WriteLine(                                     //#L
                    $"   CONSTRAINT [{table.PrimaryKey.Name}]"+        //#L
                    $" PRIMARY KEY ([" +                               //#L
                    string.Join(", ",                                  //#L
                        table.PrimaryKey.Columns.Select(x => x.Name)) +//#L
                    "])");                                             //#L
                _output.WriteLine(")\n");                              //#M
            }
            /*******************************************************************
            #A I get an instance of the DesignTimeBuilder. If there are any errors the Errors property of this instance will contain the error messages
            #B This returns a service provider for the scaffolding design-time services
            #C I now ask for the IDatabaseModelFactory service, which contains a method that reads a database an return an IModel result
            #D I use the service's Create method that takes in a connection string to the database I want to look at
            #E These two parameters allow me to pick specific tables and/or schemas respectively. An empty list means return all tables for all schemas
            #F I now loop through each tabel found in the database
            #G This outputs the CREATE TABLE command in SQL Server syntax
            #H I then loop through each column in the table
            #I This forms the correct string for a primary key, or an empty string if the column is not a priamry key
            #J This forms the NOT NULL/NULL part of the column definition
            #K I now output the whole column definition
            #L I add the primary key constraint on the end
            #M I end here. I could have output the foreign key constraints too, but to save space I didn't bother with that.
             * ****************************************************************/
        }

        [Fact]
        public void TestScaffoldingSuggestedEntityNames()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer, false);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(_connectionString, new string[] { }, new string[] { }, false);

            //ATTEMPT 
            var entities = model?.GetEntityTypes();

            //VERIFY
            entities.Select(x => x.Name).ShouldEqual(new []{ "Authors", "BookAuthor", "Books", "LineItem", "Orders", "PriceOffers", "Review" });
        }

        [Fact]
        public void TestScaffoldingWithSingularizerSuggestedEntityNames()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(_connectionString, new string[] { }, new string[] { }, false);

            //ATTEMPT 
            var entities = model?.GetEntityTypes();

            //VERIFY
            entities.OrderBy(x => x.Name).Select(x => x.Name).ShouldEqual(new[] { "Author", "Book", "BookAuthor", "LineItem", "Order", "PriceOffer", "Review" });
        }


        [Fact]
        public void ListDatabaseScalarEntityProperties()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(_connectionString, new string[] { }, new string[] { }, false);
            var entity = model?.GetEntityTypes().FirstOrDefault(x => x.Name == "Book");

            //ATTEMPT
            foreach (var prop in entity.GetProperties())
            {
                _output.WriteLine($"{prop.Name}: MaxLength = {prop.GetMaxLength()}, IsNullable = {prop.IsNullable}, IsUnicode = {prop.IsUnicode()}");
                foreach (var annotation in prop.GetAnnotations())
                {
                    _output.WriteLine($"    Annotation = {annotation.Name},{annotation.Value}");
                }
            }
        }

        [Fact]
        public void ListModelScalarEntityProperties()
        {
            //SETUP

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var entity = context.Model.GetEntityTypes().FirstOrDefault(x => x.Name == typeof(Book).FullName);
                foreach (var prop in entity.GetProperties())
                {
                    _output.WriteLine($"{prop.Name}: MaxLength = {prop.GetMaxLength()}, IsNullable = {prop.IsNullable}, IsUnicode = {prop.IsUnicode()}");
                    foreach (var annotation in prop.GetAnnotations())
                    {
                        _output.WriteLine($"    Annotation = {annotation.Name},{annotation.Value}");
                    }
                }
            }
        }

        [Fact]
        public void GetScalarEntityPropertyInModel()
        {
            //SETUP
            var builder = new DesignTimeBuilder();
            var serviceProvider = builder.GetScaffolderService(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(_connectionString, new string[] { }, new string[] { }, false);
            var entity = model?.GetEntityTypes().FirstOrDefault(x => x.Name == nameof(Book));

            //ATTEMPT
            var asciiProp = entity?.GetProperties().SingleOrDefault(x => x.Name == nameof(Book.ImageUrl));
            var annotations = asciiProp.GetAnnotations();

            //VERIFY
            ((bool)annotations.Single(x => x.Name == "Unicode").Value).ShouldBeFalse();
        }
    }
}
