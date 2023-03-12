using System.Net.Sockets;
using System.Net;
using System.Text;

namespace backend.Models.Hardware
{
    public class PicoWSocket
    {
        private byte[] _buffer = new byte[1024];
        private readonly int _port;
        private Socket? _socket;


        public string BoardId { get; }
        public readonly string Address;

        public PicoWSocket(string boardId, string address, int port)
        {
            BoardId = boardId;
            Address = address;
            _port = port;
        }

        public async Task<bool> Connect()
        {
            try
            {
                IPAddress parsedAddress = IPAddress.Parse(this.Address);
                IPEndPoint ipEndPoint = new IPEndPoint(parsedAddress, this._port);

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
            return (this._socket != null) && (this._socket.Connected);
        }

        public async Task<long> Send(string Param, string Message)
        {
            if (!this.IsConnected())
            {
                return 0;
            }

            var msg = $"{Param}->{Message}";
            Console.WriteLine($"Sending {msg}");
            var msgBytes = Encoding.UTF8.GetBytes(msg);

            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            await this._socket!.SendAsync(msgBytes, SocketFlags.None);
            Console.WriteLine($"Sent {msg}");

            return timestamp;
        }


        public async Task<Tuple<string, long>> Receive(string ExpectedParam)
        {
            if (!this.IsConnected())
            {
                Console.WriteLine($"[Pico_W-{this.BoardId}] Receive failed: socket is null or not connected");
                throw new SocketException();
            }

            var receivedByteCount = await _socket!.ReceiveAsync(this._buffer, SocketFlags.None);
            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

            var received = Encoding.UTF8.GetString(this._buffer, 0, receivedByteCount);
            Console.WriteLine($"Received {received}");

            var splitted = received.Split("->");
            var param = splitted[0];

            if (!ExpectedParam.Equals(param))
            {
                Console.WriteLine($"[Pico_W-{this.BoardId}] Receive failed: was expecting {ExpectedParam} param, got {param}.");
                throw new SocketException();
            }

            Console.WriteLine("Returning...");
            return Tuple.Create(splitted[1], timestamp);
        }
    }

}
