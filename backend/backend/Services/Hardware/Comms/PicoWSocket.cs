using System.Net.Sockets;
using System.Net;
using System.Text;

namespace backend.Services.Hardware.Comms
{
    public class PicoWSocket
    {
        private byte[] _buffer = new byte[1024];
        private readonly int _port;
        private Socket? _socket;


        public Guid BoardId { get; }
        public readonly string Address;

        public PicoWSocket(Guid boardId, string address, int port)
        {
            BoardId = boardId;
            Address = address;
            _port = port;
        }

        public async Task<bool> Connect()
        {
            try
            {
                IPAddress parsedAddress = IPAddress.Parse(Address);
                IPEndPoint ipEndPoint = new IPEndPoint(parsedAddress, _port);

                _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await _socket.ConnectAsync(ipEndPoint);

                return _socket.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to Pico board");
                Console.WriteLine(ex.ToString());
                _socket?.Close();
                return false;
            }
        }

        public bool IsConnected()
        {
            return _socket != null && _socket.Connected;
        }

        public async Task<long> Send(string Param, string Message)
        {
            if (!IsConnected())
            {
                return 0;
            }

            var msg = $"{Param}->{Message}";
            Console.WriteLine($"Sending {msg}");
            var msgBytes = Encoding.UTF8.GetBytes(msg);

            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            await _socket!.SendAsync(msgBytes, SocketFlags.None);
            Console.WriteLine($"Sent {msg}");

            return timestamp;
        }


        public async Task<Tuple<string, long>> Receive(string ExpectedParam)
        {
            if (!IsConnected())
            {
                Console.WriteLine($"[Pico_W-{BoardId}] Receive failed: socket is null or not connected");
                throw new SocketException();
            }

            var receivedByteCount = await _socket!.ReceiveAsync(_buffer, SocketFlags.None);
            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

            var received = Encoding.UTF8.GetString(_buffer, 0, receivedByteCount);
            Console.WriteLine($"Received {received}");

            var splitted = received.Split("->");
            var param = splitted[0];

            if (!ExpectedParam.Equals(param))
            {
                Console.WriteLine($"[Pico_W-{BoardId}] Receive failed: was expecting {ExpectedParam} param, got {param}.");
                throw new SocketException();
            }

            Console.WriteLine("Returning...");
            return Tuple.Create(splitted[1], timestamp);
        }
    }

}
