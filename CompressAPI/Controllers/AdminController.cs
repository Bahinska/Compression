using Microsoft.AspNetCore.Mvc;
using SensorApi.Services;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel user)
        {
            if (!await _userService.RegisterUserAsync(user))
            {
                return BadRequest(new { message = "User with the same username or email already exists" });
            }

            if (!Guid.TryParse(user.SensorId, out var sensoreId))
            {
                return BadRequest(new { message = "Invalid Sensor Id" });
            }

            return Ok(new { message = "User registered successfully" });
        }
    }
}
