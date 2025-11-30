using AutoMapper;
using Cyviz.Core.Application.DTOs.DeviceCommand;
using Cyviz.Core.Application.Exceptions;
using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Application.Services;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Infrastructure.Services
{
    public class DeviceCommandService(
        IDeviceRepository deviceRepo,
        IDeviceCommandRepository commandRepo,
        ICommandPipeline pipeline,
        IMapper mapper) : IDeviceCommandService
    {
        private readonly IDeviceRepository _deviceRepo = deviceRepo;
        private readonly IDeviceCommandRepository _commandRepo = commandRepo;
        private readonly ICommandPipeline _pipeline = pipeline;
        private readonly IMapper _mapper = mapper;

        public async Task<CommandResponseDto> CreateCommandAsync(
            string deviceId,
            CommandRequestDto request,
            CancellationToken ct = default)
        {
            var device = await _deviceRepo.GetByIdAsync(deviceId)
                ?? throw new Exception("Device not found");

            // Idempotency check
            var existing = await _commandRepo.GetByIdempotencyAsync(deviceId, request.IdempotencyKey);
            if (existing != null)
            {
                return new CommandResponseDto
                {
                    CommandId = existing.Id,
                    Status = existing.Status
                };
            }

            var command = new DeviceCommand
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                Command = request.Command,
                IdempotencyKey = request.IdempotencyKey,
                Status = CommandStatus.Pending,
                CreatedUtc = DateTime.UtcNow
            };

            await _commandRepo.AddAsync(command);
            await _commandRepo.SaveChangesAsync();

            // Try enqueue
            var accepted = await _pipeline.EnqueueAsync(command, ct);
            // Queue full, controller maps this to HTTP 429 + Retry-After
            if (!accepted)
                throw new CommandQueueFullException();

            return new CommandResponseDto
            {
                CommandId = command.Id,
                Status = command.Status
            };
        }

        public async Task<CommandStatusDto> GetCommandStatusAsync(string deviceId, Guid commandId)
        {
            var cmd = await _commandRepo.GetByIdAsync(commandId)
                ?? throw new Exception("Command not found");

            if (!string.Equals(cmd.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Command does not belong to this device");

            return new CommandStatusDto
            {
                CommandId = cmd.Id,
                Status = cmd.Status,
                CreatedUtc = cmd.CreatedUtc,
                CompletedUtc = cmd.CompletedUtc
            };
        }
    }
}
