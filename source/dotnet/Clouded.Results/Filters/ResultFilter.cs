using Clouded.Results.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Clouded.Results.Filters;

public class ResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is CloudedException cloudedException)
        {
            context.HttpContext.Response.StatusCode = cloudedException.StatusCode;
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(
                new CloudedExceptionResult
                {
                    Errors = new[]
                    {
                        new ExceptionData
                        {
                            I18N = cloudedException.I18N,
                            Error = cloudedException.Message,
                            Type = cloudedException.GetType().Name
                        }
                    }
                }
            );
        }
        else if (context.Exception != null)
        {
            // context.ExceptionHandled = !app.Environment.IsDevelopment();
            context.Result = new ObjectResult(
                new CloudedExceptionResult
                {
                    Errors = new[]
                    {
                        new ExceptionData
                        {
                            I18N = CloudedException.GeneralI18NMessage,
                            Error = CloudedException.GeneralMessage,
                            Type = context.Exception.GetType().Name
                        }
                    }
                }
            );
        }
        else if (context.Result is ObjectResult objectResult)
        {
            context.Result = new ObjectResult(
                new CloudedOkResult<object?> { Data = objectResult.Value, }
            );
        }
    }
}
