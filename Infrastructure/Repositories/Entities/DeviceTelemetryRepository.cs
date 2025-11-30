using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceTelemetryRepository(ApplicationDbContext context) : BaseRepository<DeviceTelemetry>(context), IDeviceTelemetryRepository
    {
        public async Task AddTelemetryAsync(DeviceTelemetry telemetry)
        {
            await _context.DeviceTelemetry.AddAsync(telemetry);
        }

        public async Task<IReadOnlyList<DeviceTelemetry>> GetRecentAsync(string deviceId, int limit)
        {
            if (limit <= 0) limit = 1;
            if (limit > 50) limit = 50;

            return await _context.DeviceTelemetry
                .Where(t => t.DeviceId == deviceId)
                .OrderByDescending(t => t.TimestampUtc)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DeviceTelemetry?> GetLatestAsync(string deviceId)
        {
            return await _context.DeviceTelemetry
                .Where(t => t.DeviceId == deviceId)
                .OrderByDescending(t => t.TimestampUtc)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task TrimHistoryAsync(string deviceId, int maxEntries = 50)
        {
            var toDelete = await _context.DeviceTelemetry
                .Where(t => t.DeviceId == deviceId)
                .OrderByDescending(t => t.TimestampUtc)
                .Skip(maxEntries)
                .ToListAsync();

            if (toDelete.Count == 0)
                return;

            _context.DeviceTelemetry.RemoveRange(toDelete);
        }
    }
}
