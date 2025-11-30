using Serilog;
using System.Net;
using System.Text.Json;

namespace Cyviz.Infrastructure.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private readonly Serilog.ILogger _logger = Log.ForContext<ExceptionMiddleware>();

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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString()
                                ?? Guid.NewGuid().ToString();

            var path = context.Request.Path;
            var method = context.Request.Method;

            // Structured logging with correlation ID (Required by challenge)
            _logger.Error(ex,
                "Unhandled exception while processing {Method} {Path} — CorrelationId: {CorrelationId}",
                method, path, correlationId);

            // Build safe error response
            var response = new
            {
                error = "Internal Server Error",
                message = "An unexpected error occurred.",
                correlationId
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }
    }
}
