using backend.Models.Hardware;
using System.Net.WebSockets;

namespace backend.Services.Boards.Comms
{
    class BoardEventsService
    {
        private WebSocket? _websocket;
        private TaskCompletionSource<object>? _socketFinishedTcs;

        public void StartListeningForAll(List<PicoWBoard> boards, WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            if (boards.Any(board => !board.IsConnected()))
            {
                Console.WriteLine("Cannot start listening for all events: not all boards are connected.");
                return;
            }

            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;

            // listen for events?
            // + process/return them?
        }
    }
}
