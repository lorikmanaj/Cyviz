using AutoMapper;
using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Application.Services;

namespace Cyviz.Infrastructure.Services
{
    public class DeviceService(IDeviceRepository repo, IMapper mapper) : IDeviceService
    {
        private readonly IDeviceRepository _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<KeysetPageResult<DeviceListDto>> GetDevicesAsync(string? after, int pageSize)
        {
            var page = await _repo.GetDevicesKeysetAsync(after, pageSize);

            return new KeysetPageResult<DeviceListDto>
            {
                Items = _mapper.Map<IEnumerable<DeviceListDto>>(page.Items),
                NextCursor = page.NextCursor
            };
        }

        public async Task<DeviceDetailDto> GetDeviceByIdAsync(string id)
        {
            var device = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Device not found");

            return _mapper.Map<DeviceDetailDto>(device);
        }

        public async Task UpdateDeviceAsync(string id, DeviceUpdateDto dto)
        {
            var device = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Device not found");

            _mapper.Map(dto, device);

            _repo.Update(device);
            await _repo.SaveChangesAsync();
        }
    }
}
