using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace Cyviz.Simulators
{
    public class DeviceSimulator
    {
        private readonly string _hubUrl;
        private readonly string _apiKey;

        public DeviceSimulator(string hubUrl, string apiKey)
        {
            _hubUrl = hubUrl;
            _apiKey = apiKey;
        }

        public async Task RunAsync(string deviceId, CancellationToken ct = default)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.Headers.Add("X-Api-Key", _apiKey);
                })
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("ExecuteCommand", async json =>
            {
                var cmd = JsonSerializer.Deserialize<DeviceCommandMsg>(json)!;
                await HandleCommandAsync(connection, deviceId, cmd, ct);
            });

            await connection.StartAsync(ct);
            await connection.InvokeAsync("RegisterDevice", deviceId, ct);

            _ = HeartbeatLoop(connection, deviceId, ct);
            _ = TelemetryLoop(connection, deviceId, ct);
        }

        private async Task HeartbeatLoop(HubConnection conn, string deviceId, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(5000, ct);
                await conn.InvokeAsync("Heartbeat", deviceId, ct);
            }
        }

        private async Task TelemetryLoop(HubConnection conn, string deviceId, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(2000, ct);

                string json = DeviceTelemetryGenerator.GenerateJson();

                await conn.InvokeAsync("PushTelemetry",
                    deviceId,
                    json,
                    DateTime.UtcNow,
                    ct);
            }
        }

        private async Task HandleCommandAsync(
            HubConnection conn,
            string deviceId,
            DeviceCommandMsg cmd,
            CancellationToken ct)
        {
            switch (cmd.Command.ToLowerInvariant())
            {
                case "ping":
                    await Task.Delay(200, ct);
                    await SendCompleted(conn, deviceId, cmd.CommandId, "pong", ct);
                    break;

                case "reboot":
                    await Task.Delay(3000, ct);
                    await SendCompleted(conn, deviceId, cmd.CommandId, "ok", ct);
                    break;

                default:
                    await SendCompleted(conn, deviceId, cmd.CommandId, "unknown", ct);
                    break;
            }
        }

        private Task SendCompleted(
            HubConnection conn,
            string deviceId,
            Guid cmdId,
            string result,
            CancellationToken ct)
        {
            return conn.InvokeAsync("CommandCompleted",
                deviceId,
                cmdId,
                $@"{{ ""result"": ""{result}"" }}",
                ct);
        }
    }
}
