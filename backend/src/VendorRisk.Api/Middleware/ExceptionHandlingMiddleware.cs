using Microsoft.AspNetCore.Mvc;

namespace VendorRisk.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API exception.");

            var (status, title) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid operation"),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "Invalid argument"),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = context.Request.Path
            });
        }
    }
}
