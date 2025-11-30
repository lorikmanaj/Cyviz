using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Infrastructure.Repositories.Entities
{
    public class DeviceRepository(ApplicationDbContext context) : BaseRepository<Device>(context), IDeviceRepository
    {
    }
}
