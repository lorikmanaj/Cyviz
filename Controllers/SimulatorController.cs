using Cyviz.Simulators;
using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers
{
    [ApiController]
    [Route("api/sim")]
    public class SimulatorController : ControllerBase
    {
        private readonly EdgeDeviceRunner _runner;

        public SimulatorController(EdgeDeviceRunner runner)
        {
            _runner = runner;
        }

        [HttpPost("start/{deviceId}")]
        public async Task<IActionResult> StartDevice(string deviceId)
        {
            _ = _runner.StartDeviceAsync(deviceId); // fire-and-forget
            return Ok($"Simulator for {deviceId} started.");
        }
    }
}
