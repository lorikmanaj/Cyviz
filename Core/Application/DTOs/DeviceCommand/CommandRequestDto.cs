namespace Cyviz.Core.Application.DTOs.DeviceCommand
{
    public class CommandRequestDto
    {
        public string Command { get; set; }
        public string IdempotencyKey { get; set; }
    }
}
