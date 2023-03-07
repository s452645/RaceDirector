using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System;

namespace backend.Services
{
    public class PicoWBoard
    {
        private string _address { get; }
        private int _port { get; }
        private Socket? _syncSocket { get; set; }

        public PicoWBoard(string address)
        {
            this._address = address;
            this._port = 80;
        }

        public async Task<bool> Connect()
        {
            try
            { 
                IPAddress parsedAddress = IPAddress.Parse("192.168.1.10");
                IPEndPoint ipEndPoint = new IPEndPoint(parsedAddress, 80);

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
                Byte[] bytes = Encoding.UTF8.GetBytes("sync");
                await _syncSocket?.SendAsync(bytes, SocketFlags.None);


                var buffer = new byte[1024];
                var receivedByteCount = await _syncSocket.ReceiveAsync(buffer, SocketFlags.None);
                string returnData = Encoding.UTF8.GetString(buffer, 0, receivedByteCount);

                if (!returnData.Equals("ready"))
                {
                    return null;
                }

                Thread.Sleep(100);

                bytes = Encoding.UTF8.GetBytes("[1]->-");
                var t1 = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                await _syncSocket.SendAsync(bytes, SocketFlags.None);

                bytes = Encoding.UTF8.GetBytes($"[2]->{t1}");
                receivedByteCount = await _syncSocket.SendAsync(bytes, SocketFlags.None);
                await _syncSocket.ReceiveAsync(buffer, SocketFlags.None);

                var t4 = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

                bytes = Encoding.UTF8.GetBytes($"[3]->{t4}");
                await _syncSocket.SendAsync(bytes, SocketFlags.None);


                receivedByteCount = await _syncSocket.ReceiveAsync(buffer, SocketFlags.None);
                return Encoding.UTF8.GetString(buffer, 0, receivedByteCount);
            } 
            catch (Exception e)
            {
                Console.WriteLine("Error while syncing");
                Console.WriteLine(e.ToString());
                return null;
            }
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

        public async void SyncAll(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
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
                var task = Task.Run(() =>
                {
                    return Encoding.UTF8.GetBytes(board.Sync()?.ToString() ?? "");
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
