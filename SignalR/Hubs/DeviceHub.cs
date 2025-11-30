using Cyviz.Core.Application.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Cyviz.SignalR.Hubs
{
    public class DeviceHub(
        ILogger<DeviceHub> logger,
        IDeviceTelemetryService telemetryService,
        IDeviceService deviceService) : Hub
    {
        private readonly ILogger<DeviceHub> _logger = logger;
        private readonly IDeviceTelemetryService _telemetryService = telemetryService;
        private readonly IDeviceService _deviceService = deviceService;

        /// <summary>
        /// Called when device connects. They must call JoinDevice(deviceId).
        /// </summary>
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Device connected: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// A device joins its own SignalR group = deviceId.
        /// This allows server → device routing.
        /// </summary>
        public async Task JoinDevice(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            _logger.LogInformation("Device {DeviceId} joined group.", deviceId);

            await Clients.Caller.SendAsync("Joined", deviceId);
        }

        /// <summary>
        /// Edge device sends telemetry payload (raw JSON).
        /// </summary>
        public async Task Telemetry(string deviceId, string telemetryJson)
        {
            var timestamp = DateTime.UtcNow;

            _logger.LogInformation("Telemetry received from {Device}: {Json}",
                deviceId, telemetryJson);

            await _telemetryService.AddTelemetryAsync(deviceId, telemetryJson, timestamp);

            // forward telemetry to operators connected to ControlHub
            await Clients.Group(deviceId).SendAsync("TelemetryUpdate", new
            {
                deviceId,
                timestampUtc = timestamp,
                payload = telemetryJson
            });
        }

        /// <summary>
        /// Device acknowledges a command completed.
        /// </summary>
        public async Task CommandCompleted(string deviceId, Guid commandId)
        {
            _logger.LogInformation("Device {DeviceId} completed command {CommandId}",
                deviceId, commandId);

            await Clients.Group(deviceId).SendAsync("DeviceCommandCompleted", new
            {
                deviceId,
                commandId
            });
        }

        /// <summary>
        /// Heartbeat from device.
        /// </summary>
        public async Task Heartbeat(string deviceId)
        {
            _logger.LogDebug("Heartbeat from {DeviceId}", deviceId);

            await _deviceService.MarkDeviceOnlineAsync(deviceId);
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            _logger.LogWarning("Device disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
