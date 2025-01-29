using Microsoft.AspNetCore.Mvc;
using ServerAPI.Services;

namespace SensorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private const string DemoUsername = "test";
        private const string DemoPassword = "password";

        [HttpPost("token")]
        public IActionResult GetToken([FromForm] string username, [FromForm] string password)
        {
            if (username == DemoUsername && password == DemoPassword)
            {
                var token = TokenService.GenerateJwtToken(username, "aVeryLongSecretKeyThatIsAtLeast32BytesLong", "Your_Issuer", "Your_Audience");
                return Ok(new { token = token });
            }

            return Unauthorized();
        }
    }
}