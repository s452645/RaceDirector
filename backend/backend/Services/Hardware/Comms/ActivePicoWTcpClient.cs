using backend.Utils;
using System.Net;
using System.Net.Sockets;

namespace backend.Services.Hardware.Comms
{
    public enum ConnectionType
    {
        SYNC,
        EVENTS,
        ADD_SENSORS,
    }

    public class ActivePicoWTcpClient
    {
        private readonly string _boardName;
        private readonly IPEndPoint _ipEndPoint;

        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;

        private byte[] _buffer = new byte[1024];

        public ActivePicoWTcpClient(string boardName, string address, int port)
        {
            _boardName = boardName;

            IPAddress parsedAddress = IPAddress.Parse(address);
            _ipEndPoint = new IPEndPoint(parsedAddress, port);
        }

        public async Task Connect(ConnectionType connectionType)
        {
            _tcpClient = new();
            await _tcpClient.ConnectAsync(_ipEndPoint);

            _networkStream = _tcpClient.GetStream();

            var serverMessage = await Receive("hello");
            var isServerReady = serverMessage.Content.Message.Equals("ready");

            if (!isServerReady)
            {
                throw new Exception($"Connect [{_boardName}] Pico W Board failed: no server-ready confirmation");
            }

            var connectionTypeString = GetConnectionTypeString(connectionType);
            await Send("conn-type", connectionTypeString);
            await Receive("conn-type");
        }

        public void Close()
        {
            if (!IsConnected())
            {
                throw new Exception($"Dispose [{_boardName}] Pico W Board failed: board not connected");
            }

            ForceClose();
        }

        public void ForceClose()
        {
            _networkStream?.Close();
            _tcpClient?.Close();
        }

        public bool IsConnected()
        {
            return _tcpClient != null &&
                _networkStream != null &&
                _tcpClient.Connected;
        }

        public async Task<PicoWBoardMessage> Send(string param, string message)
        {
            if (!IsConnected())
            {
                throw new Exception($"Could not send message to [{_boardName}] Pico W Board: board not connected");
            }

            var msg = new PicoWBoardMessageContent(param, message);
            LogBeforeSend(msg);

            var timestamp = TimeUtil.GetCurrentTimestamp();
            await _networkStream!.WriteAsync(msg.AsBytes());

            LogAfterSend(msg);
            return new PicoWBoardMessage(PicoWBoardMessageSender.BACKEND, timestamp, null, msg);
        }


        public async Task<PicoWBoardMessage> Receive(string expectedParam)
        {
            if (!IsConnected())
            {
                throw new Exception($"Could not receive message from [{_boardName}] Pico W Board: board not connected");
            }

            LogBeforeReceive(expectedParam);

            var receivedBytesCount = await _networkStream!.ReadAsync(_buffer);
            var timestamp = TimeUtil.GetCurrentTimestamp();
            var msg = PicoWBoardMessageContent.FromBytes(_buffer, receivedBytesCount);

            LogAfterReceive(msg);

            if (!expectedParam.Equals(msg.Param))
            {
                throw new Exception($"Receive from [{_boardName}] failed: expected {expectedParam} param, got {msg.Param}");
            }

            return new PicoWBoardMessage(PicoWBoardMessageSender.BOARD, null, timestamp, msg);
        }

        private string GetConnectionTypeString(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.SYNC:
                    return "SYNC";
                case ConnectionType.EVENTS:
                    return "EVENTS";
                case ConnectionType.ADD_SENSORS:
                    return "ADD_SENSORS";
                default:
                    throw new Exception("Invalid connection type");
            }
        }

        private void LogBeforeSend(PicoWBoardMessageContent messageToSend)
        {
            Console.WriteLine($"Sending {messageToSend.AsString()} to [{_boardName}] Pico W Board");
        }

        private void LogAfterSend(PicoWBoardMessageContent messageSent)
        {
            Console.WriteLine($"Sent {messageSent.AsString()} to [{_boardName}] Pico W Board");
        }

        private void LogBeforeReceive(string expectedParam)
        {
            Console.WriteLine($"Waiting for message with [{expectedParam}] param from [{_boardName}] Pico W Board");
        }

        private void LogAfterReceive(PicoWBoardMessageContent messageReceived)
        {
            Console.WriteLine($"Received {messageReceived.AsString()} from [{_boardName}] Pico W Board");
        }
    }
}