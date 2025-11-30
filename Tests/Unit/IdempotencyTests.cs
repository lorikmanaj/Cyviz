using AutoMapper;
using Cyviz.Core.Application.DTOs.DeviceCommand;
using Cyviz.Core.Application.Exceptions;
using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;
using Cyviz.Infrastructure.Services;
using Moq;
using Xunit;

namespace Cyviz.Tests.Unit
{
    public class IdempotencyTests
    {
        private DeviceCommandService CreateService(
    out Mock<IDeviceRepository> deviceRepo,
    out Mock<IDeviceCommandRepository> commandRepo,
    out Mock<ICommandPipeline> pipeline)
        {
            deviceRepo = new Mock<IDeviceRepository>();
            commandRepo = new Mock<IDeviceCommandRepository>();
            pipeline = new Mock<ICommandPipeline>();

            var mapper = new Mock<IMapper>();

            var service = new DeviceCommandService(
                deviceRepo.Object,
                commandRepo.Object,
                pipeline.Object,
                mapper.Object);

            return service;
        }

        // ----------------------------------------------------------------------
        // 1. First command -> should be created
        // ----------------------------------------------------------------------
        [Fact]
        public async Task CreatesNewCommand_WhenNoDuplicateExists()
        {
            // Arrange
            var service = CreateService(out var deviceRepo, out var commandRepo, out var pipeline);

            var deviceId = "device1";
            var key = "abc-123";

            deviceRepo.Setup(r => r.GetByIdAsync(deviceId))
                .ReturnsAsync(new Device { Id = deviceId });

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceId, key))
                .ReturnsAsync((DeviceCommand)null);

            pipeline.Setup(p => p.EnqueueAsync(It.IsAny<DeviceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new CommandRequestDto
            {
                Command = "turnOn",
                IdempotencyKey = key
            };

            // Act
            var result = await service.CreateCommandAsync(deviceId, request);

            // Assert
            commandRepo.Verify(r => r.AddAsync(It.Is<DeviceCommand>(c =>
                c.DeviceId == deviceId &&
                c.IdempotencyKey == key &&
                c.Status == CommandStatus.Pending
            )), Times.Once);

            commandRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotEqual(Guid.Empty, result.CommandId);
            Assert.Equal(CommandStatus.Pending, result.Status);
        }

        // ----------------------------------------------------------------------
        // 2. Second command -> should return existing command
        // ----------------------------------------------------------------------
        [Fact]
        public async Task ReturnsExistingCommand_WhenIdempotencyKeyMatches()
        {
            // Arrange
            var service = CreateService(out var deviceRepo, out var commandRepo, out var pipeline);

            var deviceId = "device1";
            var key = "abc-123";

            var existingCmd = new DeviceCommand
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                IdempotencyKey = key,
                Status = CommandStatus.Completed
            };

            deviceRepo.Setup(r => r.GetByIdAsync(deviceId))
                .ReturnsAsync(new Device { Id = deviceId });

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceId, key))
                .ReturnsAsync(existingCmd);

            var request = new CommandRequestDto
            {
                Command = "turnOn",
                IdempotencyKey = key
            };

            // Act
            var result = await service.CreateCommandAsync(deviceId, request);

            // Assert
            Assert.Equal(existingCmd.Id, result.CommandId);
            Assert.Equal(existingCmd.Status, result.Status);

            commandRepo.Verify(r => r.AddAsync(It.IsAny<DeviceCommand>()), Times.Never);
            pipeline.Verify(p => p.EnqueueAsync(It.IsAny<DeviceCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ----------------------------------------------------------------------
        // 3. Same key but different device -> new command
        // ----------------------------------------------------------------------
        [Fact]
        public async Task AllowsSameIdempotencyKey_WhenDeviceIsDifferent()
        {
            // Arrange
            var service = CreateService(out var deviceRepo, out var commandRepo, out var pipeline);

            var deviceA = "deviceA";
            var deviceB = "deviceB";
            var key = "abc-123";

            deviceRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Device());

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceA, key))
                .ReturnsAsync((DeviceCommand)null);

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceB, key))
                .ReturnsAsync((DeviceCommand)null);

            pipeline.Setup(p => p.EnqueueAsync(It.IsAny<DeviceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new CommandRequestDto { Command = "turnOn", IdempotencyKey = key };

            // Act
            var resultA = await service.CreateCommandAsync(deviceA, request);
            var resultB = await service.CreateCommandAsync(deviceB, request);

            // Assert
            Assert.NotEqual(resultA.CommandId, resultB.CommandId);
        }

        // ----------------------------------------------------------------------
        // 4. Queue full -> throw CommandQueueFullException
        // ----------------------------------------------------------------------
        [Fact]
        public async Task Throws_WhenQueueFull()
        {
            // Arrange
            var service = CreateService(out var deviceRepo, out var commandRepo, out var pipeline);

            var deviceId = "device1";
            var key = "abc-123";

            deviceRepo.Setup(r => r.GetByIdAsync(deviceId))
                .ReturnsAsync(new Device { Id = deviceId });

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceId, key))
                .ReturnsAsync((DeviceCommand)null);

            pipeline.Setup(p => p.EnqueueAsync(It.IsAny<DeviceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // QUEUE FULL

            var request = new CommandRequestDto { Command = "turnOn", IdempotencyKey = key };

            // Act + Assert
            await Assert.ThrowsAsync<CommandQueueFullException>(() =>
                service.CreateCommandAsync(deviceId, request));
        }

        // ----------------------------------------------------------------------
        // 5. Command is created with correct properties
        // ----------------------------------------------------------------------
        [Fact]
        public async Task SetsCorrectProperties_OnCreatedCommand()
        {
            // Arrange
            var service = CreateService(out var deviceRepo, out var commandRepo, out var pipeline);

            var deviceId = "device1";
            var key = "abc-123";

            deviceRepo.Setup(r => r.GetByIdAsync(deviceId))
                .ReturnsAsync(new Device { Id = deviceId });

            commandRepo.Setup(r => r.GetByIdempotencyAsync(deviceId, key))
                .ReturnsAsync((DeviceCommand)null);

            pipeline.Setup(p => p.EnqueueAsync(It.IsAny<DeviceCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var capturedCommand = new DeviceCommand();

            commandRepo.Setup(r => r.AddAsync(It.IsAny<DeviceCommand>()))
                       .Callback<DeviceCommand>(cmd => capturedCommand = cmd);

            var request = new CommandRequestDto
            {
                Command = "turnOn",
                IdempotencyKey = key
            };

            // Act
            await service.CreateCommandAsync(deviceId, request);

            // Assert
            Assert.Equal(deviceId, capturedCommand.DeviceId);
            Assert.Equal(key, capturedCommand.IdempotencyKey);
            Assert.Equal("turnOn", capturedCommand.Command);
            Assert.Equal(CommandStatus.Pending, capturedCommand.Status);
            Assert.NotEqual(default, capturedCommand.CreatedUtc);
        }
    }
}
