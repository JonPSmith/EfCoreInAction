using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using test.EfHelpers;
using Test.Chapter14Listings.EfServices;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_CreateDesignTimeBuilder
    {
        private readonly ITestOutputHelper _output;


        [Fact]
        public void CreateDesignTimeProvider()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection();
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var builder = new DesignTimeBuilder();
                //ATTEMPT 
                var serviceProvider = builder.GetScaffolderService(context);

                //VERIFY
                serviceProvider.ShouldNotBeNull();
                serviceProvider.ShouldBeType<ServiceProvider>();
            }
        }


        [Fact]
        public void GetIDatabaseModelFactory()
        {
            //SETUP
            var optionsBuilder = this.SetupOptionsWithCorrectConnection();
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var builder = new DesignTimeBuilder();
                //ATTEMPT 
                var serviceProvider = builder.GetScaffolderService(context);

                //ATTEMPT 
                var factory = serviceProvider.GetService<IDatabaseModelFactory>();

                //VERIFY
                factory.ShouldNotBeNull();
                factory.ShouldBeType<SqlServerDatabaseModelFactory>();
            }
        }
    }
}
