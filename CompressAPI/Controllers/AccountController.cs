using Microsoft.AspNetCore.Mvc;
using SensorApi.Services;
using ServerAPI.Services;

namespace SensorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromForm] string username, [FromForm] string password)
        {
            var user = await _userService.GetUserAsync(username);
            if (user != null && user.Password == password)
            {
                var token = TokenService.GenerateJwtToken(user.Username, "aVeryLongSecretKeyThatIsAtLeast32BytesLong", "Your_Issuer", "Your_Audience");
                return Ok(new { token = token });
            }

            return Unauthorized();
        }
    }
}