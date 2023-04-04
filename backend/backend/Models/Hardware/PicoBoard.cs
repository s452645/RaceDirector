namespace backend.Models.Hardware
{
    public class PicoBoard
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public List<BreakBeamSensor> BreakBeamSensors { get; set; }

        // TODO: type?
    }
}
