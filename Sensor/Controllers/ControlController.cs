using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sensor.Services;

namespace SensorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        private readonly WebSocketClient _webSocketClient;
        private readonly IConfiguration _configuration;

        public ControlController(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
        }

        //[HttpPost("start")]
        //public async Task<IActionResult> StartStreaming()
        //{
        //    await _webSocketClient.ConnectAsync();
        //    return Ok("Started streaming.");
        //}

        [HttpPost("stop")]
        public async Task<IActionResult> StopStreaming()
        {
            await _webSocketClient.DisconnectAsync();
            return Ok("Stopped streaming.");
        }
    }
}