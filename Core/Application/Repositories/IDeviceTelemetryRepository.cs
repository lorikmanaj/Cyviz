using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Core.Application.Repositories
{
    public interface IDeviceTelemetryRepository : IBaseRepository<DeviceTelemetry>
    {
        Task AddTelemetryAsync(DeviceTelemetry telemetry);
        Task<IReadOnlyList<DeviceTelemetry>> GetRecentAsync(string deviceId, int limit);
        Task<DeviceTelemetry?> GetLatestAsync(string deviceId);

        Task TrimHistoryAsync(string deviceId, int maxEntries = 50);
    }
}
