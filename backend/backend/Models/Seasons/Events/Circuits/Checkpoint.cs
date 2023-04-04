using backend.Models.Hardware;

namespace backend.Models.Seasons.Events.Circuits
{
    public enum CheckpointType
    {
        Start,
        Stop,
        Pause,
        Resume,
        Continue,
    }

    public class Checkpoint
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public CheckpointType Type { get; set; }

        public Guid? BreakBeamSensorId { get; set; }
        public BreakBeamSensor? BreakBeamSensor { get; set; }

        public Guid CircuitId { get; set; }
        public Circuit Circuit { get; set; }

    }
}
