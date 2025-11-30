using Cyviz.Core.Application.DTOs.DeviceSnapshot;
using Cyviz.Core.Application.DTOs.Telemetry;

namespace Cyviz.Core.Application.Services
{
    public interface IDeviceTelemetryService
    {
        Task AddTelemetryAsync(string deviceId, string dataJson, DateTime timestampUtc);
        Task<IReadOnlyList<TelemetryDto>> GetRecentTelemetryAsync(string deviceId, int limit);
        DeviceSnapshotDto? GetSnapshot(string deviceId);
    }
}
