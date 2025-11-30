using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;
using Cyviz.Core.Domain.Services;
using Cyviz.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cyviz.SignalR.Workers
{
    public class WorkerManager(
        ICommandPipeline pipeline,
        IServiceScopeFactory scopeFactory,
        IDeviceProtocolAdapterResolver adapterResolver,
        IDeviceCircuitBreakerRegistry circuitBreakers,
        IRetryPolicy retryPolicy,
        IHubContext<ControlHub> controlHub,
        ILogger<WorkerManager> logger) : BackgroundService
    {
        private readonly ICommandPipeline _pipeline = pipeline;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IRetryPolicy _retryPolicy = retryPolicy;
        private readonly IDeviceProtocolAdapterResolver _adapterResolver = adapterResolver;
        private readonly IDeviceCircuitBreakerRegistry _circuitBreakers = circuitBreakers;
        private readonly ILogger<WorkerManager> _logger = logger;
        private readonly IHubContext<ControlHub> _controlHub = controlHub;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkerManager starting with {WorkerCount} workers", _pipeline.WorkerCount);

            var tasks = new List<Task>();

            for (int i = 0; i < _pipeline.WorkerCount; i++)
            {
                int workerIndex = i;
                tasks.Add(RunWorkerAsync(workerIndex, stoppingToken));
            }

            return Task.WhenAll(tasks);
        }

        private async Task RunWorkerAsync(int workerIndex, CancellationToken ct)
        {
            _logger.LogInformation("Worker {WorkerIndex} started", workerIndex);

            try
            {
                await foreach (var command in _pipeline.ReadAllAsync(workerIndex, ct))
                {
                    try
                    {
                        await ProcessCommandAsync(command, ct);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        _logger.LogInformation("Worker {WorkerIndex} cancellation requested", workerIndex);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error processing command {CommandId} for device {DeviceId}",
                            command.Id, command.DeviceId);

                        // Best effort: mark as failed
                        await MarkCommandFailedAsync(command, "Unhandled worker exception");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker {WorkerIndex} stopped due to cancellation", workerIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker {WorkerIndex} crashed", workerIndex);
            }

            _logger.LogInformation("Worker {WorkerIndex} exiting", workerIndex);
        }

        private async Task ProcessCommandAsync(DeviceCommand command, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

            // 1) Load device
            var device = await deviceRepo.GetByIdAsync(command.DeviceId);
            if (device == null)
            {
                _logger.LogWarning("Device {DeviceId} not found for command {CommandId}", command.DeviceId, command.Id);
                await MarkCommandFailedAsync(command, "Device not found");
                return;
            }

            // 2) Circuit breaker check (per device)
            var breaker = _circuitBreakers.GetCircuitBreaker(device.Id);

            if (!breaker.CanExecute())
            {
                _logger.LogWarning("Circuit breaker OPEN for device {DeviceId}, command {CommandId} blocked",
                    device.Id, command.Id);

                await MarkCommandFailedAsync(command, "Circuit breaker open");
                return;
            }

            // 3) Resolve protocol adapter for this device
            var adapter = _adapterResolver.ResolveAdapter(device);

            // 4) Execute with retry + jitter
            //var success = await ExecuteWithRetryAsync(async () =>
            //{
            //    // This is where we “send” to the device
            //    await adapter.SendCommandAsync(device, command, ct);
            //}, ct);
            var success = await _retryPolicy.ExecuteAsync(
                () => adapter.SendCommandAsync(device, command, ct),
                _logger,
                ct
            );

            if (success)
            {
                breaker.OnSuccess();

                await MarkCommandCompletedAsync(command);

                // Broadcast to operator hub that command completed
                await _controlHub.Clients
                    .Group(device.Id) // e.g. group per device
                    .SendAsync("CommandCompleted", new
                    {
                        deviceId = device.Id,
                        commandId = command.Id,
                        status = CommandStatus.Completed.ToString()
                    }, cancellationToken: ct);
            }
            else
            {
                breaker.OnFailure();

                await MarkCommandFailedAsync(command, "Max retries exceeded");

                await _controlHub.Clients
                    .Group(device.Id)
                    .SendAsync("CommandFailed", new
                    {
                        deviceId = device.Id,
                        commandId = command.Id,
                        status = CommandStatus.Failed.ToString()
                    }, cancellationToken: ct);
            }
        }

        /// <summary>
        /// Retry with jitter: ~100ms, 300ms, 700ms
        /// </summary>

        private async Task MarkCommandCompletedAsync(DeviceCommand command)
        {
            using var scope = _scopeFactory.CreateScope();

            var commandRepo = scope.ServiceProvider.GetRequiredService<IDeviceCommandRepository>();

            command.Status = CommandStatus.Completed;
            command.CompletedUtc = DateTime.UtcNow;

            await commandRepo.UpdateAsync(command);
        }

        private async Task MarkCommandFailedAsync(DeviceCommand command, string reason)
        {
            using var scope = _scopeFactory.CreateScope();

            var commandRepo = scope.ServiceProvider.GetRequiredService<IDeviceCommandRepository>();

            command.Status = CommandStatus.Failed;
            command.CompletedUtc = DateTime.UtcNow;
            // Optionally add: command.ErrorMessage = reason;

            await commandRepo.UpdateAsync(command);
        }
    }
}
