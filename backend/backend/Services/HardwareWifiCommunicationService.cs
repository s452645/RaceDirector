using backend.Models.Hardware;
using System.Net.WebSockets;

namespace backend.Services
{
    public class HardwareWifiCommunicationService
    {
        private TimeSyncService _timeSyncService;
        private WifiEventsService _wifiEventsService;
        private List<PicoWBoard> _picoWBoards = new List<PicoWBoard>();

        HardwareWifiCommunicationService(TimeSyncService timeSyncService, WifiEventsService wifiEventsService)
        {
            _timeSyncService = timeSyncService;
            _wifiEventsService = wifiEventsService;
        }

        // TODO: currently for every port (sync, events) on the same board there have to be another instance of Board
        // Maybe it should be one?
        public async Task<bool> AddAndConnectBoard(PicoWBoard picoWBoard)
        {
            var result = await picoWBoard.Connect();

            if (!result) 
            {
                Console.WriteLine("Board failed to connect");
                return false;
            }

            _picoWBoards.Add(picoWBoard);
            return true;
        }

        public void SyncAllForever(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _timeSyncService.StartSyncingAll(_picoWBoards, webSocket, socketFinishedTcs);
        }

        public void HandleAllEventsForever(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _wifiEventsService.StartListeningForAll(_picoWBoards, webSocket, socketFinishedTcs);
        }
    }
}
