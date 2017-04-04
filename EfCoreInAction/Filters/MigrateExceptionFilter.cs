using Autofac.Core;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EfCoreInAction.Filters
{
    public class MigrateExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public MigrateExceptionFilter(
            IHostingEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            //ASP.NET Core DI returns OutstandingMigrationException
            //while AutoFac returns DependencyResolutionException, with the InnerExeption.InnerException being OutstandingMigrationException
            if (!(context.Exception is OutstandingMigrationException ||
                (context.Exception is DependencyResolutionException 
                && context.Exception?.InnerException?.InnerException is OutstandingMigrationException))) return;

            var result = new ViewResult
            {
                ViewName = "DatabaseError"
            };
            context.Result = result;
        }
    }
}