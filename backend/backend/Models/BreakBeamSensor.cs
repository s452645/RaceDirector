namespace backend.Models
{
    public class BreakBeamSensor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PicoBoard Board { get; set; }
        public int Pin { get; set; }
    }
}
