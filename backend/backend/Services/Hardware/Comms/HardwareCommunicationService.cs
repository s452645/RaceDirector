using NuGet.Protocol.Plugins;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace backend.Services.Hardware.Comms
{
    public class HardwareCommunicationService
    {
        public readonly static string defaultHostName = "defaultHostName";
        public readonly static int defaultPort = 6655;

        private IPEndPoint? endpoint;
        private Socket? socket;

        private byte[] bytes = new byte[1024];
        private bool isConnected = false;

        public HardwareCommunicationService()
        {
            IPHostEntry host = Dns.GetHostEntry(defaultHostName);
            Connect(host.AddressList.ToList());
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
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while connecting to {socket?.RemoteEndPoint}: {e.Message}");
                }
            }

            return isConnected;
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

        public string ReceiveMessage()
        {
            if (!isConnected || socket == null)
            {
                return "";
            }

            int bytesReceived = socket.Receive(bytes);
            string messageReceived = Encoding.UTF8.GetString(bytes, 0, bytesReceived);

            bytes = new byte[1024];

            return messageReceived;
        }
    }
}
