using Microsoft.AspNetCore.Mvc;

namespace Cyviz.Controllers.HealthCheck
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        //Demo
        [HttpGet]
        public IActionResult Health() => Ok(new { status = "Healthy" });
    }
}
