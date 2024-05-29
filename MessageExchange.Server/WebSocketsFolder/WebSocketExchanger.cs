using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace MessageExchange.Server.WebSocketsFolder
{
    public class WebSocketExchanger : IExchanger
    {
        public ConcurrentDictionary<string, WebSocket> WebSockets { get; private set; }

        public WebSocketExchanger() 
        {
            WebSockets = new();
        }

        public async Task AddReceiver(HttpContext HttpContext)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketId = Guid.NewGuid().ToString();
                WebSockets.TryAdd(socketId, webSocket);
                await Echo(webSocket, socketId);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        public async Task SendMessageToAll(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var tasks = WebSockets.Values.Select(async webSocket =>
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        private async Task Echo(WebSocket webSocket, string socketId)
        {
            var buffer = new byte[128 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            WebSockets.TryRemove(socketId, out _);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
