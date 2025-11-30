using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceCommandRepository(ApplicationDbContext context) : BaseRepository<DeviceCommand>(context), IDeviceCommandRepository
    {
        public async Task<DeviceCommand?> GetByIdAsync(Guid id)
        {
            return await _context.DeviceCommands
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<DeviceCommand?> GetByIdempotencyAsync(string deviceId, string idempotencyKey)
        {
            return await _context.DeviceCommands
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.DeviceId == deviceId &&
                    c.IdempotencyKey == idempotencyKey);
        }

        public async Task<bool> ExistsIdempotentAsync(string deviceId, string idempotencyKey)
        {
            return await _context.DeviceCommands
                .AnyAsync(c => c.DeviceId == deviceId && c.IdempotencyKey == idempotencyKey);
        }
    }
}
