using Cyviz.Core.Domain.Entities;

namespace Cyviz.Core.Application.Interfaces
{
    public interface ICommandPipeline
    {
        Task<bool> EnqueueAsync(DeviceCommand command, CancellationToken cancellationToken = default);
        IAsyncEnumerable<DeviceCommand> ReadAllAsync(int workerIndex, CancellationToken ct);
    }
}
