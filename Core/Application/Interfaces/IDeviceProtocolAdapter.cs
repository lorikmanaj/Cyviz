using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.ValueObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Cyviz.Core.Application.Interfaces
{
    public interface IDeviceProtocolAdapter
    {
        Task ConnectAsync(Device device, CancellationToken token);
        Task<CommandResult> SendCommandAsync(Device device, Command command, CancellationToken token);
        IAsyncEnumerable<TelemetryPayload> StreamTelemetryAsync(Device device, CancellationToken token);
    }
}
