using backend.Models.Hardware;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.Services.Hardware.Comms
{
    public class BoardEventWebsocketObserver : IBoardEventsObserver
    {
        private readonly WebSocket _websocket;
        private readonly TaskCompletionSource<object> _socketFinishedTcs;

        public BoardEventWebsocketObserver(WebSocket websocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _websocket = websocket;
            _socketFinishedTcs = socketFinishedTcs;
        }

        public async Task Notify(BoardEvent boardEvent)
        {
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(boardEvent));
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