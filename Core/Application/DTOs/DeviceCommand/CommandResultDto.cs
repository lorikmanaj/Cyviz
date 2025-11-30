namespace Cyviz.Core.Application.DTOs.DeviceCommand
{
    public class CommandResultDto
    {
        public Guid CommandId { get; set; }
        public string Status { get; set; } = default!;
        public DateTime SubmittedUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
    }
}
