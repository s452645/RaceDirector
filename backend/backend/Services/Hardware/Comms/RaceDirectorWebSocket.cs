using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.Services.Hardware.Comms
{
    public class RaceDirectorWebSocket<T>
    {
        private readonly WebSocket _websocket;
        private readonly TaskCompletionSource<object> _socketFinishedTcs;

        public RaceDirectorWebSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;
        }

        public async Task Send(T data)
        {
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            var byteArraySegment = new ArraySegment<byte>(responseBytes, 0, responseBytes.Length);

            if (_websocket != null)
            {
                await _websocket.SendAsync(
                    byteArraySegment,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }
    }
}
