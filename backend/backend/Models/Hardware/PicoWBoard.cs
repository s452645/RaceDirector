namespace backend.Models.Hardware
{
    public class PicoWBoard
    {
        public static readonly int SYNC_PORT = 12000;
        public static readonly int EVENT_PORT = 15000;

        public PicoWSocket SyncSocket { get; }
        public PicoWSocket EventSocket { get; }
        public string Id { get; }


        public PicoWBoard(string id, string address)
        {
            this.Id = id;
            this.SyncSocket = new PicoWSocket(this.Id, address, SYNC_PORT);
            this.EventSocket = new PicoWSocket(this.Id, address, EVENT_PORT);
        }

        public async Task<bool> ConnectSockets()
        {
            var syncConnected = await this.SyncSocket.Connect();
            var eventConnected = await this.EventSocket.Connect();

            return syncConnected && eventConnected;
        }

        public bool IsConnected()
        {
            return this.SyncSocket.IsConnected() && this.EventSocket.IsConnected();
        }
    }
}
