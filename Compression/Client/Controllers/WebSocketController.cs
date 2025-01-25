using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace SensorClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly WebSocketHandler _webSocketHandler;

        public WebSocketController(IWebHostEnvironment environment, WebSocketHandler webSocketHandler)
        {
            _environment = environment;
            _webSocketHandler = webSocketHandler;
        }

        [HttpGet("/ws/client")]
        public async Task GetClient()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _webSocketHandler.Handle(HttpContext);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartStreaming()
        {
            await _webSocketHandler.Broadcast(Encoding.UTF8.GetBytes("start"));
            return Ok("Streaming started.");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopStreaming()
        {
            await _webSocketHandler.Broadcast(Encoding.UTF8.GetBytes("stop"));
            return Ok("Streaming stopped.");
        }
    }
}