using Serilog;
using System.Text;

namespace Cyviz.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private readonly Serilog.ILogger _logger = Log.ForContext<RequestLoggingMiddleware>();

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var requestBody = await ReadRequestBody(context.Request);
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            await _next(context);

            var responseText = await ReadResponseBody(context.Response);
            await responseBody.CopyToAsync(originalBodyStream);

            _logger.Information("HTTP {Method} {Path} responded {StatusCode} - CorrelationId: {CorrelationId}, Request: {RequestBody}, Response: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId,
                requestBody,
                responseText);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body.Length > 5000 ? "(truncated)" : body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text.Length > 5000 ? "(truncated)" : text;
        }
    }
}
