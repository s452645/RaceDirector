namespace backend.Models
{
    public class Checkpoint
    {
        public Guid Id { get; set; }
        public BreakBeamSensor BreakBeamSensor { get; set; }
        public Circuit Circuit { get; set; }
        
        // TODO: type?
    }
}
