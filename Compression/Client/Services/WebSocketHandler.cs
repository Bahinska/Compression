using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;

public class WebSocketHandler
{
    private readonly ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();
    private const string clientSocket = "clientSocket";

    public async Task Handle(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            _connections.TryAdd(clientSocket, webSocket);

            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _connections.TryRemove(clientSocket, out _);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> messageHandler)
    {
        var buffer = new byte[1024 * 512];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
            messageHandler(result, buffer);
            //await Broadcast(buffer);
        }
    }

    public async Task Broadcast(byte[] buffer)
    {
        foreach (var connection in _connections)
        {
            if (connection.Value.State == WebSocketState.Open)
            {
                await connection.Value.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}