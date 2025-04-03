using FluentValidation;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Stockas.Application.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An exception occurred");

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            Type = $"http://localhost:5192/errors/{context.Response.StatusCode}",
            Title = exception switch
            {
                ValidationException => "Validation Error",
                UnauthorizedAccessException => "Unauthorized",
                KeyNotFoundException => "Not Found",
                ArgumentException => "Bad Request",
                _ => "Internal Server Error",
            },
            Status = context.Response.StatusCode,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Errors = exception is ValidationException validationException
                ? validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
                : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }
}
