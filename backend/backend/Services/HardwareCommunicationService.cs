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

        private IPEndPoint? endpoint;
        private Socket? socket;

        private byte[] bytes = new byte[1024];
        private bool isConnected = false;

        public HardwareCommunicationService()
        {
            IPHostEntry host = Dns.GetHostEntry(defaultHostName);
            this.Connect(host.AddressList.ToList());
        }

        private bool Connect(List<IPAddress> iPAddresses)
        {
            List<IPAddress> addressesToCheck = iPAddresses;

            while (!this.isConnected && addressesToCheck.Count > 0)
            {
                var ipAddress = addressesToCheck[0];
                addressesToCheck.RemoveAt(0);

                try
                {
                    this.endpoint = new IPEndPoint(ipAddress, defaultPort);
                    this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    this.socket.Connect(endpoint);
                    Console.WriteLine($"Socket connected to {socket.RemoteEndPoint}");
                    this.isConnected = true;
                } catch (Exception e)
                {
                    Console.WriteLine($"Error while connecting to {socket?.RemoteEndPoint}: {e.Message}");
                }
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
