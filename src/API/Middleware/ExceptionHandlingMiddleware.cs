using System.Net;
using FluentValidation;

namespace SimpleLibrary.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning($"ValidationException: {ex.Message}");
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            var response = new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed.",
                Errors = errors
            };

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await context.Response.WriteAsync(jsonResponse);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning($"ArgumentException: {ex.Message}");
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (FormatException ex)
        {
            logger.LogWarning($"FormatException: {ex.Message}");
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"KeyNotFoundException: {ex.Message}");
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError($"Unexpected error: {ex.Message}");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            context.Response.StatusCode,
            Message = message
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(jsonResponse);
    }
}