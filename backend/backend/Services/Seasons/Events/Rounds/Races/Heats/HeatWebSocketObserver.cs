using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.Services.Seasons.Events.Rounds.Races.Heats
{
    public class HeatWebSocketObserver : IHeatObserver
    {

        private readonly WebSocket _websocket;
        private readonly TaskCompletionSource<object> _socketFinishedTcs;

        public HeatWebSocketObserver(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;
        }

        public async void Notify(SeasonEventRoundRaceHeatDto newHeatState)
        {
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newHeatState));
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
