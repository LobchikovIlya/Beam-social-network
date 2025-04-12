using System.Net;
using System.Text.Json;
using Beam.Core.Exceptions;
using Beam.Shared.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Beam.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
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
            context.Response.ContentType = "application/json";

            // Логирование ошибки
            _logger.LogError(exception, "Unhandled exception occurred.");

            // Обработка ошибок на основе типа
            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToArray();
                var result = JsonSerializer.Serialize(errors);
                await context.Response.WriteAsync(result);
            }

            else if (exception is ExceptionBase customException)
            {
                context.Response.StatusCode = (int)customException.HttpStatusCode;
                var errorResponse = _env.IsDevelopment()
                    ? new ErrorResponse(customException.Message, customException.StackTrace)
                    : new ErrorResponse(customException.Message);

                var result = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = _env.IsDevelopment()
                    ? new ErrorResponse("Internal Server Error", exception.StackTrace)
                    : new ErrorResponse("Internal Server Error");

                var result = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(result);
            }
        }
    }
}