using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Services;
using Cyviz.Core.Domain.ValueObjects;

namespace Cyviz.Infrastructure.ProtocolAdapters_PLACEHOLDERS_
{
    /// <summary>
    /// Adapter that uses your own SignalR "edge" connection as the transport.
    /// The edge device connects to /devicehub and joins a group (e.g. device.Id).
    /// Commands are sent via SignalR to that group, and the edge responds.
    /// </summary>
    public class EdgeSignalRAdapter(ILogger<EdgeSignalRAdapter> logger) : IEdgeSignalRAdapter
    {
        private readonly ILogger<EdgeSignalRAdapter> _logger = logger;

        public Task ConnectAsync(Device device, CancellationToken token)
        {
            // Connection is actually from edge → server, so nothing to do here.
            _logger.LogInformation("SignalR edge adapter 'connecting' to device {DeviceId}", device.Id);
            return Task.CompletedTask;
        }

        public async Task<CommandResult> SendCommandAsync(Device device, DeviceCommand command, CancellationToken token)
        {
            _logger.LogInformation("Dispatching SignalR command '{Command}' to edge device {DeviceId}", command.Command, device.Id);

            // Real flow:
            // - Use IHubContext<DeviceHub> to send "ExecuteCommand" to the device group
            // - Edge agent runs it and sends back "CommandCompleted"

            await Task.Delay(50, token);

            return CommandResult.Ok();
        }

        public async IAsyncEnumerable<TelemetryPayload> StreamTelemetryAsync(Device device, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
        {
            // Real flow:
            // - DeviceHub would receive telemetry from edge devices
            // - This adapter is more conceptual; you might not call StreamTelemetryAsync
            while (!token.IsCancellationRequested)
            {
                yield return new TelemetryPayload
                {
                    TimestampUtc = DateTime.UtcNow,
                    PayloadJson = """{ "status": "OK", "source": "edge-signalr" }"""
                };

                await Task.Delay(TimeSpan.FromSeconds(2), token);
            }
        }
    }
}
