using backend.Models.Dtos.Hardware;

namespace backend.Services.Hardware.Comms
{
    public class ActivePicoWBoard
    {
        public static readonly int COMMS_PORT = 15000;

        public Guid BoardId { get; }

        private PicoBoardDto _picoBoard { get; }
        private ActivePicoWTcpClient _boardTcpClient { get; }

        public ActivePicoWBoard(PicoBoardDto picoBoard)
        {
            BoardId = picoBoard.Id;
            _picoBoard = picoBoard;
            _boardTcpClient = new ActivePicoWTcpClient(picoBoard.Name, picoBoard.IPAddress, COMMS_PORT);
        }

        public async Task Connect(ConnectionType connectionType)
        {
            await _boardTcpClient.Connect(connectionType);
        }

        public void Disconnect()
        {
            _boardTcpClient.Close();
        }

        public bool IsConnected()
        {
            return _boardTcpClient.IsConnected();
        }

        public async Task<PicoWBoardMessage> Send(string param, string message)
        {
            return await _boardTcpClient.Send(param, message);
        }

        public async Task<PicoWBoardMessage> Receive(string expectedParam)
        {
            return await _boardTcpClient.Receive(expectedParam);
        }
    }
}
