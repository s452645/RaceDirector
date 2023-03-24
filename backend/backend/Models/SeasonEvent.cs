namespace backend.Models
{
    public class SeasonEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid? CircuitId { get; set; }
        public Circuit? Circuit { get; set; }


        public Guid SeasonId { get; set; }
        public Season Season { get; set; }

        // TODO: type?
        // TODO: rounds
    }
}
