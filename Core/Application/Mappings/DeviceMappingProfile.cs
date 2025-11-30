using AutoMapper;
using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Domain.Entities;

namespace Cyviz.Core.Application.Mappings
{
    public class DeviceMappingProfile : Profile
    {
        public DeviceMappingProfile()
        {
            CreateMap<Device, DeviceListDto>();
            CreateMap<Device, DeviceDetailDto>();
            CreateMap<DeviceUpdateDto, Device>();
        }
    }
}
