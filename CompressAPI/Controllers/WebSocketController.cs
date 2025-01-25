using Microsoft.AspNetCore.Mvc;


namespace SensorApi.Controllers
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

        [HttpGet("/ws/sensor")]
        public async Task GetSensor()
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
            await _webSocketHandler.StartStreaming();
            return Ok("Streaming started.");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopStreaming()
        {
            await _webSocketHandler.StopStreaming();
            return Ok("Streaming stopped.");
        }
    }
}