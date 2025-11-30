using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Domain.Entities;

namespace Cyviz.Core.Application.Services
{
    public interface IDeviceService
    {
        Task<KeysetPageResult<DeviceListDto>> GetDevicesAsync(string? after, int pageSize);
        Task<DeviceDetailDto> GetDeviceByIdAsync(string id);
        Task UpdateDeviceAsync(string id, DeviceUpdateDto dto);

        Task UpdateDeviceAsync(string id, DeviceUpdateDto dto, byte[] ifMatchRowVersion);
        Task AppendTelemetryAsync(string deviceId, DeviceTelemetry telemetry);
    }
}
