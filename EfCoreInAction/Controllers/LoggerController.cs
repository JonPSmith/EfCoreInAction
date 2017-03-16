using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Logger;

namespace EFCoreExample.Controllers
{
    public class LoggerController : Controller
    {
        [HttpGet]
        public JsonResult GetLog(string traceIdentifier)
        {
            return Json(HttpRequestLog.GetHttpRequestLog(traceIdentifier));
        }
    }
}
