using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceTelemetryRepository(ApplicationDbContext context) : BaseRepository<DeviceTelemetry>(context), IDeviceTelemetryRepository
    {
        public async Task TrimHistoryAsync(string deviceId, int maxEntries = 50)
        {
            var oldEntries = _context.DeviceTelemetry
                .Where(t => t.DeviceId == deviceId)
                .OrderByDescending(t => t.TimestampUtc)
                .Skip(maxEntries);

            _context.DeviceTelemetry.RemoveRange(oldEntries);

            await _context.SaveChangesAsync();
        }
    }
}
