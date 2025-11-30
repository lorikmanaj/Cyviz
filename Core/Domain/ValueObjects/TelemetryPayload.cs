namespace Cyviz.Core.Domain.ValueObjects
{
    public class TelemetryPayload
    {
        public DateTime TimestampUtc { get; init; }
        public string PayloadJson { get; init; } = default!;
    }
}
