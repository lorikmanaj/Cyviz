using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Extensions;
using Cyviz.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceRepository(ApplicationDbContext context) : BaseRepository<Device>(context), IDeviceRepository
    {
        public async Task<KeysetPageResult<Device>> GetDevicesKeysetAsync(string? after, int pageSize)
        {
            IQueryable<Device> query = _context.Devices.AsNoTracking();

            // Cursor conversion: if null -> pass null
            return await query.ToKeysetPageAsync(
                keySelector: d => d.Id,  // Keyset: device-01, device-02
                after: string.IsNullOrWhiteSpace(after) ? null : after,
                pageSize: pageSize
            );
        }
    }
}
