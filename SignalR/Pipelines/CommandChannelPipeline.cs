using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Domain.Entities;
using System.Threading.Channels;

///
/// Requirements:Bounded channel per worker

//Size = 50

//DropWrite mode

//So TryWrite returns false, enabling HTTP 429 behavior.
//Consistent hashing

//Ensures command ordering & correct affinity.

//Supports graceful shutdown

//Workers can use ReadAllAsync with cancellation.
/// 
///

namespace Cyviz.SignalR.Pipelines
{
    public class CommandChannelPipeline : ICommandPipeline
    {
        private readonly int _workerCount;
        private readonly Channel<DeviceCommand>[] _channels;

        public CommandChannelPipeline(int? workerCount = null)
        {
            // default workers = min(4, CPU/2)
            _workerCount = workerCount ?? Math.Max(2, Math.Min(4, Environment.ProcessorCount / 2));

            _channels = new Channel<DeviceCommand>[_workerCount];

            for (int i = 0; i < _workerCount; i++)
            {
                _channels[i] = Channel.CreateBounded<DeviceCommand>(
                    new BoundedChannelOptions(50)
                    {
                        FullMode = BoundedChannelFullMode.DropWrite, // So TryWrite returns false
                        SingleReader = true,
                        SingleWriter = false
                    });
            }
        }

        // Determine which worker owns this device
        private int GetWorkerIndex(string deviceId)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in deviceId)
                    hash = hash * 31 + c;

                return Math.Abs(hash % _workerCount);
            }
        }

        // Called by DeviceCommandService
        public async Task<bool> EnqueueAsync(DeviceCommand command, CancellationToken cancellationToken = default)
        {
            int workerIndex = GetWorkerIndex(command.DeviceId);
            var channel = _channels[workerIndex];

            // Non-blocking attempt to write
            if (channel.Writer.TryWrite(command))
                return true;

            // If immediate write failed, try with WaitToWriteAsync
            if (await channel.Writer.WaitToWriteAsync(cancellationToken))
                return channel.Writer.TryWrite(command);

            // Queue full or cancelled
            return false;
        }

        // Read: workers drain channel
        public async IAsyncEnumerable<DeviceCommand> ReadAllAsync(int workerIndex,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            var channel = _channels[workerIndex];

            while (await channel.Reader.WaitToReadAsync(ct))
            {
                while (channel.Reader.TryRead(out var cmd))
                {
                    yield return cmd;
                }
            }
        }
    }
}
