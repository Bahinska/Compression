
using Microsoft.AspNetCore.Mvc;
using Sensor.Services;

namespace SensorApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ControlController : ControllerBase
    {
        private readonly WebSocketClient _webSocketClient;

        public ControlController(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
        }

        [HttpGet("status")]
        public IActionResult st()
        {
            return Ok("Started");
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartStreaming()
        {
            await _webSocketClient.ConnectAsync();
            return Ok("Started streaming.");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopStreaming()
        {
            await _webSocketClient.DisconnectAsync();
            return Ok("Stopped streaming.");
        }
    }
}