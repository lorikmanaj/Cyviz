using Cyviz.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cyviz.Core.Domain.Entities
{
    public class Device
    {
        public string Id { get; set; } // PK

        public string Name { get; set; }

        public DeviceType Type { get; set; }

        public DeviceProtocol Protocol { get; set; }

        public List<string> Capabilities { get; set; }

        public DeviceStatus Status { get; set; }

        public DateTime LastSeenUtc { get; set; }

        public string Firmware { get; set; }

        public string Location { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } // For ETag
    }
}
