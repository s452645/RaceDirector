using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;

namespace backend.Services.Hardware.Comms
{
    public class PicoWBoard
    {
        public static readonly int SYNC_PORT = 15000;
        public static readonly int EVENT_PORT = 12000;

        public PicoWSocket SyncSocket { get; }
        public PicoWSocket EventSocket { get; }
        
        public PicoBoardDto PicoBoardDto { get; }


        public PicoWBoard(PicoBoardDto picoBoard)
        {
            var id = picoBoard.Id;
            var address = picoBoard.IPAddress;

            PicoBoardDto = picoBoard;
            SyncSocket = new PicoWSocket(id, address, SYNC_PORT);
            EventSocket = new PicoWSocket(id, address, EVENT_PORT);
        }

        public async Task<bool> ConnectSockets()
        {
            var syncConnected = await SyncSocket.Connect();
            var eventConnected = await EventSocket.Connect();

            return syncConnected && eventConnected;
        }

        public bool IsConnected()
        {
            return SyncSocket.IsConnected() && EventSocket.IsConnected();
        }
    }
}
