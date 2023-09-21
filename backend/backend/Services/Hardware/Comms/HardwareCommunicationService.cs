using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace backend.Services.Hardware.Comms
{
    public class PicoUSBMessage
    {
        public string msg { get; set; }
        public long timestamp { get; set; }
    }

    public class HardwareCommunicationService
    {
        private readonly BoardListener _boardListener;

        public readonly static string defaultHostName = "DESKTOP-RARH2F6";
        public readonly static int defaultPort = 6655;

        private IPEndPoint? endpoint;
        private Socket? socket;

        private byte[] bytes = new byte[1024];
        private bool isConnected = false;

        private List<Task> tasks = new List<Task>();

        public HardwareCommunicationService(BoardListener boardListener)
        {
            IPHostEntry host = Dns.GetHostEntry(defaultHostName);
            Connect(host.AddressList.ToList());
            _boardListener = boardListener;
        }

        private bool Connect(List<IPAddress> iPAddresses)
        {
            List<IPAddress> addressesToCheck = iPAddresses;

            while (!isConnected && addressesToCheck.Count > 0)
            {
                var ipAddress = addressesToCheck[0];
                addressesToCheck.RemoveAt(0);

                try
                {
                    endpoint = new IPEndPoint(ipAddress, defaultPort);
                    socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    socket.Connect(endpoint);
                    Console.WriteLine($"Socket connected to {socket.RemoteEndPoint}");
                    isConnected = true;

                    tasks.Add(Task.Run(async () => await ListenForMessages()));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while connecting to {socket?.RemoteEndPoint}: {e.Message}");
                }
            }

            return isConnected;
        }

        public async Task ListenForMessages()
        {
            while (true)
            {
                try
                {
                    var picoMessage = await ReceiveMessage();

                    if (picoMessage == null)
                    {
                        return;
                    }

                    if (picoMessage.msg == "car_detected")
                    {
                        await _boardListener.EmitElevatorEnterEvent(picoMessage.timestamp);
                    }
                    else if (picoMessage.msg == "release")
                    {
                        await _boardListener.EmitElevatorExitEvent(picoMessage.timestamp);
                    }
                    else if (picoMessage.msg == "start")
                    {
                        await _boardListener.EmitStartEvent(picoMessage.timestamp);
                    }
                    else if (picoMessage.msg == "sector_0")
                    {
                        await _boardListener.EmitSector0Event(picoMessage.timestamp);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public int SendMessage(string msg)
        {
            if (!isConnected || socket == null)
            {
                return 0;
            }

            byte[] message = Encoding.UTF8.GetBytes(msg);

            // returns number of sent bytes
            return socket.Send(message);
        }

        public async Task<PicoUSBMessage?> ReceiveMessage()
        {
            if (!isConnected || socket == null)
            {
                return null;
            }

            int bytesReceived = await socket.ReceiveAsync(bytes, SocketFlags.None);
            string messageReceived = Encoding.UTF8.GetString(bytes, 0, bytesReceived);

            var deserialized = JsonSerializer.Deserialize<PicoUSBMessage>(messageReceived);

            bytes = new byte[1024];

            return deserialized;
        }
    }
}