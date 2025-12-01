using Cyviz.Core.Domain.Enums;
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

    public class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
    {
        private readonly RequestDelegate _next = next;
        private readonly ApiKeyOptions _keys = options.Value;
        private readonly Serilog.ILogger _logger = Log.ForContext<ApiKeyMiddleware>();

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            // Allow CORS preflight (OPTIONS) to pass through without API key
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

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
            if (apiKey == _keys.Device)
                context.Items["CallerType"] = ApiCallerType.Device;

            else if (apiKey == _keys.Operator)
                context.Items["CallerType"] = ApiCallerType.Operator;

            else
            {
                await WriteUnauthorized(context, "Invalid API key");
                return;
            }

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
