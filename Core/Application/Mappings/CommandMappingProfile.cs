using AutoMapper;
using Cyviz.Core.Application.DTOs.DeviceCommand;
using Cyviz.Core.Domain.Entities;

namespace Cyviz.Core.Application.Mappings
{
    public class CommandMappingProfile : Profile
    {
        public CommandMappingProfile()
        {
            CreateMap<DeviceCommand, CommandStatusDto>()
                .ForMember(dest => dest.CommandId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
