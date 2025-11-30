using Microsoft.Extensions.Options;
using Serilog;
using System.Net;

namespace Cyviz.Infrastructure.Middlewares
{
    public class ApiKeyOptions
    {
        public string Device { get; set; }
        public string Operator { get; set; }
    }

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiKeyOptions _keys;
        private readonly Serilog.ILogger _logger = Log.ForContext<ApiKeyMiddleware>();

        public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
        {
            _next = next;
            _keys = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            // Allow Swagger + health checks without API key
            if (path.StartsWith("/swagger") ||
                path.StartsWith("/health") ||
                path.StartsWith("/metrics"))
            {
                await _next(context);
                return;
            }

            // Extract API key
            var apiKey =
                context.Request.Headers["X-Api-Key"].FirstOrDefault()
                ?? context.Request.Query["apiKey"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await WriteUnauthorized(context, "Missing API key");
                return;
            }

            // Validate against known keys
            if (apiKey != _keys.Device && apiKey != _keys.Operator)
            {
                await WriteUnauthorized(context, "Invalid API key");
                return;
            }

            // Bind caller type to HttpContext for logging
            context.Items["CallerType"] = apiKey == _keys.Device ? "Device" : "Operator";

            await _next(context);
        }

        private async Task WriteUnauthorized(HttpContext context, string message)
        {
            _logger.Warning("Unauthorized request to {Path}. Reason: {Reason}",
                context.Request.Path, message);

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync($@"
            {{
                ""error"": ""Unauthorized"",
                ""message"": ""{message}""
            }}");
        }
    }
}
