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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await Task.FromResult(_userService.GetAllUsers());
            return Ok(users);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserModel updatedUser)
        {
            var result = await _userService.UpdateUserAsync(id, updatedUser);
            if (result)
            {
                return Ok(new { message = "User updated successfully" });
            }

            return NotFound();
        }
    }
}
