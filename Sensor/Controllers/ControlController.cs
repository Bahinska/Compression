
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sensor.Services;

namespace SensorApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    public class ControlController : ControllerBase
    {
        private readonly WebSocketClient _webSocketClient;

        public ControlController(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartStreaming()
        {

            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var clientPort = HttpContext.Connection.RemotePort;

            // Log or use the client host information
            Console.WriteLine($"StopStreaming called by {clientIp}:{clientPort}");
            var serverUri = new Uri($"http://{clientIp}:{clientPort}");
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