using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Core.Application.Interfaces
{
    public interface IDeviceTelemetryRepository : IBaseRepository<DeviceTelemetry>
    {
        Task TrimHistoryAsync(string deviceId, int maxEntries = 50);
    }
}
