namespace backend.Models
{
    public class BreakBeamSensor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Pin { get; set; }
       
        public Guid BoardId { get; set; }
        public PicoBoard Board { get; set; }
    }
}
