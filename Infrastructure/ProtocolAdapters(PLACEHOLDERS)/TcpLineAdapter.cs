using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Services;
using Cyviz.Core.Domain.ValueObjects;

namespace Cyviz.Infrastructure.ProtocolAdapters_PLACEHOLDERS_
{
    /// <summary>
    /// Conceptual RS-232/Telnet adapter.
    /// For Extron SW HD 4K PLUS, you would:
    /// - Open TCP/serial connection
    /// - Send SIS commands like "1*!" for input select
    /// - Read "]" terminated responses and parse them
    /// </summary>
    public class TcpLineAdapter : ITcpLineAdapter
    {
        private readonly ILogger<TcpLineAdapter> _logger;

        public TcpLineAdapter(ILogger<TcpLineAdapter> logger)
        {
            _logger = logger;
        }

        public Task ConnectAsync(Device device, CancellationToken token)
        {
            // Here you open a TCP or serial connection.
            // For this test, we just log.
            _logger.LogInformation("TCP adapter connecting to device {DeviceId}", device.Id);
            return Task.CompletedTask;
        }

        public async Task<CommandResult> SendCommandAsync(Device device, DeviceCommand command, CancellationToken token)
        {
            _logger.LogInformation("Sending TCP command '{Command}' to device {DeviceId}", command.Command, device.Id);

            // In a real Extron integration:
            //  - Map command.Command ("Input1", "PowerOn") to SIS command strings ("1*!", "1%1P", etc)
            //  - Write over TCP or serial port
            //  - Read response until ']' delimiter, parse status

            await Task.Delay(50, token); // Simulate IO latency

            // For now, always succeed
            return CommandResult.Ok();
        }

        public async IAsyncEnumerable<TelemetryPayload> StreamTelemetryAsync(Device device, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
        {
            //poll the device periodically.
            while (!token.IsCancellationRequested)
            {
                yield return new TelemetryPayload
                {
                    TimestampUtc = DateTime.UtcNow,
                    PayloadJson = """{ "status": "OK", "source": "tcp-line" }"""
                };

                await Task.Delay(TimeSpan.FromSeconds(2), token);
            }
        }
    }
}
