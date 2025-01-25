using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Net.Http;

public class WebSocketHandler
{
    private readonly ConcurrentDictionary<WebSocket, WebSocket> _connections = new ConcurrentDictionary<WebSocket, WebSocket>();
    private readonly HttpClient _httpClient;
    private WebSocket _sensorConnection;

    public WebSocketHandler()
    {
        _httpClient = new HttpClient();
    }

    public async Task Handle(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (context.Request.Path == "/ws/sensor")
            {
                _sensorConnection = webSocket;
            }
            else
            {
                _connections.TryAdd(webSocket, webSocket);
            }

            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _connections.TryRemove(webSocket, out _);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });

            if (_connections.IsEmpty && _sensorConnection != null && _sensorConnection.State == WebSocketState.Open)
            {
                await _sensorConnection.CloseAsync(WebSocketCloseStatus.NormalClosure, "No active clients", CancellationToken.None);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> messageHandler)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            messageHandler(result, buffer);
        }
    }

    public async Task StartStreaming()
    {
        var response = await _httpClient.PostAsync("http://sensor-address/api/control/start", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Started streaming on sensor.");
        }
        else
        {
            Console.WriteLine("Failed to start streaming on sensor.");
        }
    }

    public async Task StopStreaming()
    {
        var response = await _httpClient.PostAsync("http://sensor-address/api/control/stop", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Stopped streaming on sensor.");
        }
        else
        {
            Console.WriteLine("Failed to stop streaming on sensor.");
        }
    }
}