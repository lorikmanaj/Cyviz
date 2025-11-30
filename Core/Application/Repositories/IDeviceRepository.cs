using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Core.Application.Repositories
{
    public interface IDeviceRepository : IBaseRepository<Device>
    {
        bool AnyDevices();
        Task<KeysetPageResult<Device>> GetDevicesKeysetAsync(string? after, int pageSize);
    }
}
