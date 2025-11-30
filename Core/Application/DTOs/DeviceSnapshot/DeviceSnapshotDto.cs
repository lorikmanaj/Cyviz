using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Application.DTOs.DeviceSnapshot
{
    public class DeviceSnapshotDto
    {
        public string DeviceId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string DataJson { get; set; }
    }
}
