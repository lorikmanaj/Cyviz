using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Application.Models.Pagination;

namespace Cyviz.Core.Application.Services
{
    public interface IDeviceService
    {
        Task<KeysetPageResult<DeviceListDto>> GetDevicesAsync(string? after, int pageSize);
        Task<DeviceDetailDto> GetDeviceByIdAsync(string id);
        Task UpdateDeviceAsync(string id, DeviceUpdateDto dto);
    }
}
