using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers.HealthCheck
{
    [ApiController]
    [Route("metrics")]
    public class MetricsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Metrics()
        {
            // In real system, aggregate metrics from WorkerManager
            return Ok(new
            {
                command_latency_p95 = 0,
                telemetry_rate = 0,
                reconnects = 0,
                circuit_breakers_open = 0
            });
        }
    }
}
