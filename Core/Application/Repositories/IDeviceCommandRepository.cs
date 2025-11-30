using Cyviz.Core.Domain.Entities;
using Cyviz.Infrastructure.Repositories.Generic;

namespace Cyviz.Core.Application.Repositories
{
    public interface IDeviceCommandRepository : IBaseRepository<DeviceCommand>
    {
        Task<bool> ExistsIdempotentAsync(string deviceId, string idempotencyKey);
    }
}
