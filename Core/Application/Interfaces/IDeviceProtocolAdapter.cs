using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.ValueObjects;

namespace Cyviz.Core.Application.Interfaces
{
    public interface IDeviceProtocolAdapter
    {
        Task ConnectAsync(Device device, CancellationToken token);
        Task<CommandResult> SendCommandAsync(Device device, DeviceCommand command, CancellationToken token);
        IAsyncEnumerable<TelemetryPayload> StreamTelemetryAsync(Device device, CancellationToken token);
    }
}
