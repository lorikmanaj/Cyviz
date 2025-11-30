using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Application.Services;
using Cyviz.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers
{
    [ApiController]
    [Route("api/devices")]
    [ApiRequireOperator]   // Only UI/Operator may access
    public class DevicesController(IDeviceService deviceService) : ControllerBase
    {
        private readonly IDeviceService _deviceService = deviceService;

        // GET /api/devices?$top=10&$after=device-05&status=&type=&search=
        [HttpGet]
        public async Task<ActionResult<KeysetPageResult<DeviceListDto>>> GetDevices(
            [FromQuery(Name = "$after")] string? after,
            [FromQuery(Name = "$top")] int top = 10)
        {
            var result = await _deviceService.GetDevicesAsync(after, top);
            return Ok(result);
        }

        // GET /api/devices/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDetailDto>> GetDevice(string id)
        {
            var dto = await _deviceService.GetDeviceByIdAsync(id);
            return Ok(dto);
        }

        // PUT /api/devices/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(
            string id,
            [FromBody] DeviceUpdateDto dto)
        {
            if (!Request.Headers.TryGetValue("If-Match", out var etagHeader))
                return StatusCode(StatusCodes.Status428PreconditionRequired,
                    new { error = "Missing If-Match header" });

            byte[] rowVersion;
            try
            {
                rowVersion = Convert.FromBase64String(etagHeader!);
            }
            catch
            {
                return BadRequest(new { error = "Invalid If-Match ETag format" });
            }

            await _deviceService.UpdateDeviceAsync(id, dto, rowVersion);
            return NoContent();
        }
    }
}
