using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sensor.Services;

namespace SensorApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ControlController : ControllerBase
    {
        private readonly WebSocketClient _webSocketClient;

        public ControlController(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartStreaming([FromBody] string uri)
        {
            // Log or use the client host information
            var serverUri = new Uri(uri);
            await _webSocketClient.ConnectAsync(serverUri);
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