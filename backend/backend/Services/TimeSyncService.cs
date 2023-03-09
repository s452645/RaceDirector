using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.WebSockets;
using System;

namespace backend.Services
{
    public class PicoWBoard
    {
        private byte[] _buffer = new byte[1024];
        private string _address { get; }
        private int _port { get; }
        private Socket? _syncSocket { get; set; }

        private string Id { get; }

        public PicoWBoard(string id, string address, int port)
        {
            this.Id = id;
            this._address = address;
            this._port = port;
        }

        public async Task<bool> Connect()
        {
            try
            { 
                IPAddress parsedAddress = IPAddress.Parse(this._address);
                IPEndPoint ipEndPoint = new IPEndPoint(parsedAddress, this._port);

                _syncSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await _syncSocket.ConnectAsync(ipEndPoint);

                return _syncSocket.Connected;
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to Pico board");
                Console.WriteLine(ex.ToString());
                _syncSocket?.Close();
                return false;
            }
        }

        public async Task<string?> Sync()
        {
            try
            {
                Byte[] bytes = Encoding.UTF8.GetBytes("[-]->sync");

                if (_syncSocket == null)
                {
                    return null;
                }

                await _syncSocket.SendAsync(bytes, SocketFlags.None);
                var response = await this.receiveMsg();
                
                if (!response.Equals("ready"))
                {
                    return null;
                }

                Thread.Sleep(100);

                bytes = Encoding.UTF8.GetBytes("[1]->-");
                var t1 = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                await _syncSocket.SendAsync(bytes, SocketFlags.None);

                bytes = Encoding.UTF8.GetBytes($"[2]->{t1}");
                await _syncSocket.SendAsync(bytes, SocketFlags.None);

                _ = await this.receiveMsg();

                var t4 = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                bytes = Encoding.UTF8.GetBytes($"[3]->{t4}");
                await _syncSocket.SendAsync(bytes, SocketFlags.None);

                return await this.receiveMsg();
            } 
            catch (Exception e)
            {
                Console.WriteLine("Error while syncing");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private async Task<string> receiveMsg()
        {
            if (this._syncSocket == null)
            {
                Console.WriteLine("Receive msg failed: socket is null");
                throw new SocketException();
            }

            var receivedByteCount = await _syncSocket.ReceiveAsync(this._buffer, SocketFlags.None);
            return Encoding.UTF8.GetString(this._buffer, 0, receivedByteCount);
        }
    }

    public class TimeSyncService
    {
        private List<PicoWBoard> _boards = new List<PicoWBoard>();
        private WebSocket? _websocket;
        private TaskCompletionSource<object>? _socketFinishedTcs;

        public void addBoard(PicoWBoard board)
        {
            _boards.Add(board);
        }

        public async void KeepSyncingAll(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;

            var tasks = _boards.Select(b => b.Connect());
            await Task.WhenAll(tasks);

            var autoEvent = new AutoResetEvent(false);

            var stateTimer = new Timer(
                SyncAllBoards,
                autoEvent, 1000, 30000);
        }

        private void SyncAllBoards(Object? stateInfo)
        {
            this._boards.ForEach(async (board) =>
            {
                // TODO: is it blocking the main thread?
                var task = Task.Run(async () =>
                {
                    var response = await board.Sync() ?? "";
                    return Encoding.UTF8.GetBytes(response);
                });

                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    byte[] response = task.Result;
                    var byteArraySegment = new ArraySegment<byte>(response, 0, response.Length);

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
            });
        }
    }
}
