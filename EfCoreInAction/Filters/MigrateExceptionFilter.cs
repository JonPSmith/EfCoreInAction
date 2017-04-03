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
            if (!((context.Exception is DependencyResolutionException) 
                && (context.Exception?.InnerException?.InnerException is OutstandingMigrationException))) return;

            var result = new ViewResult
            {
                ViewName = "DatabaseError"
            };
            context.Result = result;
        }
    }
}