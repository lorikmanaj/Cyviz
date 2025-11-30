using Microsoft.AspNetCore.SignalR;

namespace Cyviz.SignalR.Hubs
{
    public class ControlHub(ILogger<ControlHub> logger) : Hub
    {
        private readonly ILogger<ControlHub> _logger = logger;

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Control client connected: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Operators subscribe to a specific device (view page open).
        /// </summary>
        public async Task SubscribeToDevice(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            _logger.LogInformation("Client {Conn} subscribed to device {DeviceId}",
                Context.ConnectionId, deviceId);

            await Clients.Caller.SendAsync("Subscribed", deviceId);
        }

        /// <summary>
        /// Operators leave a device.
        /// </summary>
        public async Task UnsubscribeFromDevice(string deviceId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
            _logger.LogInformation("Client {Conn} unsubscribed from device {DeviceId}",
                Context.ConnectionId, deviceId);
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            _logger.LogWarning("Control client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
