using AutoMapper;
using Cyviz.Core.Application.DTOs.Telemetry;
using Cyviz.Core.Domain.Entities;

namespace Cyviz.Core.Application.Mappings
{
    public class TelemetryMappingProfile : Profile
    {
        public TelemetryMappingProfile()
        {
            CreateMap<DeviceTelemetry, TelemetryDto>();
        }
    }
}
