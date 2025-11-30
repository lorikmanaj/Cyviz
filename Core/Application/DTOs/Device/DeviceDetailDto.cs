using Cyviz.Core.Application.DTOs.DeviceSnapshot;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Application.DTOs.Device
{
    public class DeviceDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public DeviceProtocol Protocol { get; set; }
        public List<string> Capabilities { get; set; }
        public DeviceStatus Status { get; set; }
        public string Firmware { get; set; }
        public string Location { get; set; }
        public DateTime LastSeenUtc { get; set; }

        public DeviceSnapshotDto? LatestTelemetry { get; set; }
    }
}
