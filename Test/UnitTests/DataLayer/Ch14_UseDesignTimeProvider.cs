using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter14Listings.EfClasses;
using Test.Chapter14Listings.EfServices;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_UseDesignTimeProvider
    {
        private readonly ITestOutputHelper _output;
        private readonly string connectionString;
        private readonly DbContextOptions<EfCoreContext> options;

        public Ch14_UseDesignTimeProvider(ITestOutputHelper output)
        {
            _output = output;
            connectionString = this.GetUniqueDatabaseConnectionString();
            options = this.ClassUniqueDatabaseSeeded4Books();
        }


        [Fact]
        public void TestGetDatabaseModel()
        {
            //SETUP
            var builder = new DesignTimeProvider();
            var serviceProvider = builder.GetDesignTimeProvider(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IDatabaseModelFactory>();

            //ATTEMPT 
            var model = factory.Create(connectionString, new string[] { }, new string[] { });

            //VERIFY
            model.ShouldNotBeNull();
        }

        [Fact]
        public void TestDatabaseModelTableNames()
        {
            //SETUP
            var builder = new DesignTimeProvider();
            var serviceProvider = builder.GetDesignTimeProvider(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IDatabaseModelFactory>();

            var model = factory.Create(connectionString, new string[] { }, new string[] { });

            //ATTEMPT 
            var tables = model?.Tables;

            //VERIFY
            tables.Select(x => x.Name).ShouldEqual(new[] { "Authors", "Books", "Orders", "BookAuthor", "PriceOffers", "Review", "LineItem"});
        }


        [Fact]
        public void TestScaffoldingSuggestedEntityNames()
        {
            //SETUP
            var builder = new DesignTimeProvider();
            var serviceProvider = builder.GetDesignTimeProvider(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(connectionString, new string[] { }, new string[] { }, false);

            //ATTEMPT 
            var entities = model?.GetEntityTypes();

            //VERIFY
            entities.Select(x => x.Name).ShouldEqual(new []{ "Authors", "BookAuthor", "Books", "LineItem", "Orders", "PriceOffers", "Review" });
        }


        [Fact]
        public void ListDatabaseScalarEntityProperties()
        {
            //SETUP
            var builder = new DesignTimeProvider();
            var serviceProvider = builder.GetDesignTimeProvider(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(connectionString, new string[] { }, new string[] { }, false);
            var entity = model?.GetEntityTypes().FirstOrDefault(x => x.Name == "Books");

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
                var entity = context.Model.GetEntityTypes().FirstOrDefault(x => x.Name == typeof(ScalarEntity).FullName);
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
            var builder = new DesignTimeProvider();
            var serviceProvider = builder.GetDesignTimeProvider(DatabaseProviders.SqlServer);
            var factory = serviceProvider.GetService<IScaffoldingModelFactory>();

            var model = factory.Create(connectionString, new string[] { }, new string[] { }, false);
            var entity = model?.GetEntityTypes().FirstOrDefault(x => x.Name == nameof(Review));

            //ATTEMPT
            var props = entity?.GetProperties();
            var asciiProp = entity?.GetProperties().SingleOrDefault(x => x.Name == nameof(ScalarEntity.StringAscii));
            var annotations = asciiProp.GetAnnotations();

            //VERIFY
            asciiProp.ShouldNotBeNull();
        }
    }
}
