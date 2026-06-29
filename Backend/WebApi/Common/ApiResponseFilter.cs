using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Common;

/// <summary>
/// Automatically wraps all successful action results into a standardized <see cref="ApiResponse"/> format.
/// <br/>
/// - Strings and nulls → <c>{ success, message }</c><br/>
/// - Objects → <c>{ success, message, data }</c><br/>
/// - 204 No Content → skipped (preserves HTTP spec)<br/>
/// - Already-wrapped ApiResponse or ProblemDetails (validation errors) → skipped<br/>
/// - Non-2xx responses → skipped (error responses handled by ExceptionMiddleware)
/// </summary>
public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Skip if an unhandled exception occurred (ExceptionMiddleware handles it)
        if (context.Exception != null)
            return;

        switch (context.Result)
        {
            // ── ObjectResult: controller returned Ok(data), Ok("message"), BadRequest(...), etc. ──
            case ObjectResult obj:
            {
                // Skip responses already in ApiResponse format, or ProblemDetails (model validation errors)
                if (obj.Value is ApiResponse)
                    return;
#if NET8_0_OR_GREATER
                if (obj.Value is HttpValidationProblemDetails)
                    return;
#endif

                var statusCode = obj.StatusCode ?? StatusCodes.Status200OK;

                // Only wrap 2xx responses (errors are thrown as exceptions, not returned)
                if (statusCode < 200 || statusCode >= 300)
                    return;

                context.Result = obj.Value switch
                {
                    null => new ObjectResult(ApiResponse.Ok()) { StatusCode = statusCode },
                    string message => new ObjectResult(ApiResponse.Ok(message))
                    {
                        StatusCode = statusCode,
                    },
                    _ => new ObjectResult(ApiResponse<object>.Ok(obj.Value))
                    {
                        StatusCode = statusCode,
                    },
                };
                break;
            }

            // ── StatusCodeResult: controller returned Ok(), NoContent(), etc. ──
            case StatusCodeResult status:
            {
                // 204 No Content must have no body per HTTP spec
                if (status.StatusCode == StatusCodes.Status204NoContent)
                    return;

                if (status.StatusCode >= 200 && status.StatusCode < 300)
                {
                    context.Result = new ObjectResult(ApiResponse.Ok())
                    {
                        StatusCode = status.StatusCode,
                    };
                }
                break;
            }
        }
    }
}
