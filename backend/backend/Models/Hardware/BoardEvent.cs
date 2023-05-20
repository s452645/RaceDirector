namespace backend.Models.Hardware
{
    public class BoardEvent
    {
        public Guid Id { get; set; }

        public Guid SensorId { get; set; }
        public BreakBeamSensor Sensor { get; set; }
        public bool Broken { get; set; }
        public Int64 PicoLocalTimestamp { get; set; }
        public long ReceivedTimestamp { get; set; }

        public BoardEvent()
        {
            Id = Guid.NewGuid();
        }
    }
}
