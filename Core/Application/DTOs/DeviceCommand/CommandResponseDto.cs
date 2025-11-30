using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Application.DTOs.DeviceCommand
{
    public class CommandResponseDto
    {
        public Guid CommandId { get; set; }
        public CommandStatus Status { get; set; }
    }
}
