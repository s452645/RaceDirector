using NuGet.Protocol.Plugins;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace backend.Services
{
    public class HardwareCommunicationService
    {
        public readonly static string defaultHostName = "defaultHostName";
        public readonly static int defaultPort = 6655;

        private readonly IPEndPoint endpoint;
        private readonly Socket socket;

        private byte[] bytes = new byte[1024];
        private bool isConnected = false;

        public HardwareCommunicationService()
        {
            IPHostEntry host = Dns.GetHostEntry(defaultHostName);
            IPAddress ipAddress = host.AddressList[5];

            this.endpoint = new IPEndPoint(ipAddress, defaultPort);
            this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Connect()
        {
            if (this.isConnected)
            {
                return true;
            }

            try
            {
                this.socket.Connect(endpoint);
                Console.WriteLine($"Socket connected to {socket.RemoteEndPoint}");
                this.isConnected = true;
            } catch (Exception e)
            {
                Console.WriteLine($"Error while connecting to {socket.RemoteEndPoint}: {e.Message}");
                this.isConnected = false;
            }

            return this.isConnected;
        }

        public int SendMessage(string msg)
        {
            if (!this.isConnected)
            {
                return 0;
            }

            byte[] message = Encoding.UTF8.GetBytes(msg);

            // returns number of sent bytes
            return socket.Send(message);
        }

        public string ReceiveMessage()
        {
            if (!this.isConnected)
            {
                return "";
            }

            int bytesReceived = this.socket.Receive(bytes);
            string messageReceived = Encoding.UTF8.GetString(bytes, 0, bytesReceived);

            this.bytes = new byte[1024];

            return messageReceived;
        }
    }
}
