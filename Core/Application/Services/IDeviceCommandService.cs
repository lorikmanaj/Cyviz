using Cyviz.Core.Application.DTOs.DeviceCommand;

namespace Cyviz.Core.Application.Services
{
    public interface IDeviceCommandService
    {
        Task<CommandResponseDto> CreateCommandAsync(string deviceId, CommandRequestDto request, CancellationToken ct = default);
        Task<CommandStatusDto> GetCommandStatusAsync(string deviceId, Guid commandId);
    }
}
