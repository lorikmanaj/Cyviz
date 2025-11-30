namespace Cyviz.Core.Domain.Entities
{
    public class DeviceTelemetry
    {
        public long Id { get; set; } // auto-increment
        public string DeviceId { get; set; }

        public DateTime TimestampUtc { get; set; }

        public string DataJson { get; set; } // flexible structure
    }
}
