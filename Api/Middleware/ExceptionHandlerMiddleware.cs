using Api.Contracts;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

internal class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

           // await HandleExceptionAsync(httpContext, ex);
        }
    }

    //private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    //{
    //   // (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error> errors) = GetHttpStatusCodeAndErrors(exception);

    //    httpContext.Response.ContentType = "application/json";

    //    httpContext.Response.StatusCode = (int)httpStatusCode;

    //    var serializerOptions = new JsonSerializerOptions
    //    {
    //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //    };

    //    string response = JsonSerializer.Serialize(new ApiErrorResponse(errors), serializerOptions);
   
    //    await httpContext.Response.WriteAsync(response);
    //}

    //private static (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error>) GetHttpStatusCodeAndErrors(Exception exception) =>
    //    exception switch
    //    {
    //        var validationException => (HttpStatusCode.BadRequest, validationException.Errors),
    //        DomainException domainException => (HttpStatusCode.BadRequest, new[] { domainException.Error }),
    //        _ => (HttpStatusCode.InternalServerError, new[] { DomainErrors.General.ServerError })
    //    };

}

/// <summary>
/// Contains extension methods for configuring the exception handler middleware.
/// </summary>
internal static class ExceptionHandlerMiddlewareExtensions
{
    internal static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionHandlerMiddleware>();
}
