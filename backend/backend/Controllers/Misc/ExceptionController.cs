using backend.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Misc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExceptionController : Controller
    {
        [Route("/error")]
        public IActionResult HandlerErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            var statusCode = 500;
            if (exceptionHandlerFeature.Error is CustomHttpException)
            {
                statusCode = ((CustomHttpException)exceptionHandlerFeature.Error).StatusCode;
            }

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message,
                statusCode: statusCode
            );
        }
    }
}
