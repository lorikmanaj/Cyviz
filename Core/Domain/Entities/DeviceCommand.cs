using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Domain.Entities
{
    public class DeviceCommand
    {
        public Guid Id { get; set; } // commandId returned to caller
        public string DeviceId { get; set; }

        public string Command { get; set; }
        public string IdempotencyKey { get; set; }

        public CommandStatus Status { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
    }
}
