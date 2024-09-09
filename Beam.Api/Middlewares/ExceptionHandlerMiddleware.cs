
using System.Net;
using System.Text.Json;
using Beam.Core.Exceptions;
using Beam.Shared.Responses;

namespace Beam.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            HandleException(context, ex);
        }
    }

    public void HandleException(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        if (exception is ExceptionBase customException)
        {
            context.Response.StatusCode = (int)customException.HttpStatusCode;


            var errorResponse = _env.IsDevelopment()
                ? new ErrorResponse(customException.Message, customException.StackTrace)
                : new ErrorResponse(customException.Message);

            var result = JsonSerializer.Serialize(errorResponse);
            context.Response.WriteAsync(result);

        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = _env.IsDevelopment()
                ? new ErrorResponse("Internal Server Error", exception.StackTrace)
                : new ErrorResponse("Internal Server Error");
            var result = JsonSerializer.Serialize(errorResponse);
            context.Response.WriteAsync(result);
        }
    }
}