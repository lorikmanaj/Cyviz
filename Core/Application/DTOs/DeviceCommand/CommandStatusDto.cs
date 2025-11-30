using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Application.DTOs.DeviceCommand
{
    public class CommandStatusDto
    {
        public Guid CommandId { get; set; }
        public CommandStatus Status { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
    }
}
