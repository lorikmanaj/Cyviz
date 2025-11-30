using Cyviz.Core.Application.DTOs.Telemetry;
using Cyviz.Core.Application.Services;
using Cyviz.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers
{
    [ApiController]
    [Route("api/devices/{deviceId}")]
    [ApiRequireDevice]  // Only device API key may call
    public class TelemetryController(IDeviceTelemetryService telemetryService) : ControllerBase
    {
        private readonly IDeviceTelemetryService _telemetryService = telemetryService;

        // POST /api/devices/{deviceId}/telemetry
        [HttpPost("telemetry")]
        public async Task<IActionResult> AddTelemetry(
            string deviceId,
            [FromBody] TelemetryDto dto)
        {
            await _telemetryService.AddTelemetryAsync(
                deviceId,
                dto.DataJson,
                dto.TimestampUtc);

            return Accepted();
        }

        // GET /api/devices/{deviceId}/telemetry?limit=20
        [HttpGet("telemetry")]
        public async Task<ActionResult<IReadOnlyList<TelemetryDto>>> GetTelemetry(
            string deviceId,
            [FromQuery] int limit = 20)
        {
            var data = await _telemetryService.GetRecentTelemetryAsync(deviceId, limit);
            return Ok(data);
        }
    }
}
