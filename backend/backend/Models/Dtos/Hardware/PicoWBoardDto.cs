namespace backend.Models.Dtos.Hardware
{
    public class PicoWBoardDto
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public bool IsConnected { get; set; }

        public PicoWBoardDto(string id, string address, bool isConnected)
        {
            Id = id;
            Address = address;
            IsConnected = isConnected;
        }
    }
}
