using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Stockas.Application.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An exception occurred");

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            Title = exception switch
            {
                ValidationException => "Validation Error",
                UnauthorizedAccessException => "Unauthorized",
                KeyNotFoundException => "Not Found",
                _ => "Internal Server Error"
            },
            Status = context.Response.StatusCode,
            Detail = exception.Message,
            Errors = exception is ValidationException validationException
                ? validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
                : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}