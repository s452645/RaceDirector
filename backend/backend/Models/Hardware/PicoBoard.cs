namespace backend.Models.Hardware
{
    public enum PicoBoardType
    {
        USB,
        WiFi
    }

    public class PicoBoard
    {
        public Guid Id { get; set; }
        public PicoBoardType Type { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool Active { get; set; }
        public bool Connected { get; set; }
        public List<BreakBeamSensor> BreakBeamSensors { get; set; }
        public List<SyncBoardResult> SyncBoardResults { get; set; }
    }
}
