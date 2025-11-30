using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceCommandRepository(ApplicationDbContext context) : BaseRepository<DeviceCommand>(context), IDeviceCommandRepository
    {
        public async Task<bool> ExistsIdempotentAsync(string deviceId, string idempotencyKey)
        {
            return await _context.DeviceCommands
                .AnyAsync(c => c.DeviceId == deviceId && c.IdempotencyKey == idempotencyKey);
        }
    }
}
