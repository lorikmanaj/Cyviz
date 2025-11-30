using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Services;
using Cyviz.Core.Domain.ValueObjects;

namespace Cyviz.Infrastructure.ProtocolAdapters_PLACEHOLDERS_
{
    /// <summary>
    /// Conceptual HTTP/JSON adapter.
    /// Devices expose REST-ish endpoints; we POST commands, GET status, etc.
    /// </summary>
    public class HttpJsonAdapter(ILogger<HttpJsonAdapter> logger, IHttpClientFactory httpClientFactory) : IHttpJsonAdapter
    {
        private readonly ILogger<HttpJsonAdapter> _logger = logger;
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("DeviceHttpClient");

        public Task ConnectAsync(Device device, CancellationToken token)
        {
            _logger.LogInformation("HTTP adapter 'connecting' to device {DeviceId}", device.Id);
            return Task.CompletedTask;
        }

        public async Task<CommandResult> SendCommandAsync(Device device, DeviceCommand command, CancellationToken token)
        {
            _logger.LogInformation("Sending HTTP JSON command '{Command}' to device {DeviceId}", command.Command, device.Id);

            // Example (conceptual):
            // var response = await _httpClient.PostAsJsonAsync(
            //     $"devices/{device.Id}/commands",
            //     new { command = command.Command },
            //     token);

            await Task.Delay(50, token);

            // For now assume success
            return CommandResult.Ok();
        }

        public async IAsyncEnumerable<TelemetryPayload> StreamTelemetryAsync(Device device, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
        {
            // For real devices: poll /telemetry or open SSE/WebSocket, etc.
            while (!token.IsCancellationRequested)
            {
                yield return new TelemetryPayload
                {
                    TimestampUtc = DateTime.UtcNow,
                    PayloadJson = """{ "status": "OK", "source": "http-json" }"""
                };

                await Task.Delay(TimeSpan.FromSeconds(2), token);
            }
        }
    }
}
