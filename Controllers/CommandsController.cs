using Cyviz.Core.Application.DTOs.DeviceCommand;
using Cyviz.Core.Application.Services;
using Cyviz.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers
{
    [ApiController]
    [Route("api/devices/{deviceId}/commands")]
    [ApiRequireOperator]
    public class CommandsController(IDeviceCommandService commandService) : ControllerBase
    {
        private readonly IDeviceCommandService _commandService = commandService;

        // POST /api/devices/{deviceId}/commands
        [HttpPost]
        public async Task<ActionResult<CommandResponseDto>> CreateCommand(
            string deviceId,
            [FromBody] CommandRequestDto request)
        {
            var result = await _commandService.CreateCommandAsync(deviceId, request);
            return Ok(result);
        }

        // GET /api/devices/{deviceId}/commands/{commandId}
        [HttpGet("{commandId:guid}")]
        public async Task<ActionResult<CommandStatusDto>> GetStatus(
            string deviceId, Guid commandId)
        {
            var result = await _commandService.GetCommandStatusAsync(deviceId, commandId);
            return Ok(result);
        }
    }
}
