using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Application.DTOs.Device
{
    public class DeviceListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; }
        public string Location { get; set; }
        public DateTime LastSeenUtc { get; set; }
    }
}
