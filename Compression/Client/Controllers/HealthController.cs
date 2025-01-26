using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SensorClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetClient()
        {
            return Ok();
        }
    }
}
