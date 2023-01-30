using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.WebAPI.Controllers
{
    public class ErrorController: ControllerBase
    {
        // Type type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
        // private readonly log4net.ILog _log;// = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //[Route("/error-local-development")]
        //public IActionResult ErrorLocalDevelopment() => Problem(); // Add extra details here


        private readonly ILogger<ErrorController> _log;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _log = logger;
        }

       // [Route("/error-local-development")]
       // public IActionResult ErrorLocalDevelopment(
       //[FromServices] IWebHostEnvironment webHostEnvironment)
       // {
           
       //     if (webHostEnvironment.EnvironmentName != "Development")
       //     {
       //         throw new InvalidOperationException(
       //             "This shouldn't be invoked in non-development environments.");
       //     }

       //     var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

       //     _log.LogError(string.Format("Error: {0}, Stack Trace: {1}, Request: {2}",
       //           context.Error.Message,
       //           context.Error.StackTrace,
       //           context.Error.TargetSite));

       //     return Problem(
       //         detail: context.Error.StackTrace,
       //         title: context.Error.Message);


       // }

       // [Route("/error")]
       // public IActionResult Error() => Problem();
    }

    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }

    //public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    //{
    //    public int Order { get; } = int.MaxValue - 10;

    //    public void OnActionExecuting(ActionExecutingContext context) { }

    //    public void OnActionExecuted(ActionExecutedContext context)
    //    {
    //        if (context.Exception is HttpResponseException exception)
    //        {
    //            context.Result = new ObjectResult(exception.Value)
    //            {
    //                StatusCode = exception.Status,
    //            };
    //            context.ExceptionHandled = true;
    //        }
    //    }
    //}
}
