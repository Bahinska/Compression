using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Sensor.Services
{
    public class WebSocketClient
    {
        private readonly ClientWebSocket _clientWebSocket;
        private readonly Uri _serverUri;
        private bool _isStreaming;
        private bool _isConnected;

        public WebSocketClient(Uri serverUri)
        {
            _clientWebSocket = new ClientWebSocket();
            _serverUri = serverUri;
            _isStreaming = false;
            _isConnected = false;

            _clientWebSocket.ConnectAsync(_serverUri, CancellationToken.None);
            _isConnected = true;
            _isStreaming = true;
            Console.WriteLine("Connected to WebSocket server.");
            //ReceiveMessagesAsync();
        }

        //public async Task ConnectAsync()
        //{
        //    if (!_isConnected)
        //    {
        //        await _clientWebSocket.ConnectAsync(_serverUri, CancellationToken.None);
        //        _isConnected = true;
        //        _isStreaming = true;
        //        Console.WriteLine("Connected to WebSocket server.");
        //        await ReceiveMessagesAsync();
        //    }
        //}

        public async Task SendFrameAsync(Mat frame)
        {
            if (_isStreaming)
            {
                var image = frame.ToBytes(".png");
                //Console.WriteLine($"Sending frame of size: {image.Length} bytes");
                var buffer = new ArraySegment<byte>(image);

                await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024];
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (message == "start")
                    {
                        _isStreaming = true;
                        Console.WriteLine("Received start command. Beginning streaming.");
                    }
                    else if (message == "stop")
                    {
                        _isStreaming = false;
                        Console.WriteLine("Received stop command. Stopping streaming.");
                        await DisconnectAsync();
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (_isConnected)
            {
                _isStreaming = false;
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _isConnected = false;
            }
        }
    }
}