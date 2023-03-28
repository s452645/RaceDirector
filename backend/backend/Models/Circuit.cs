namespace backend.Models
{
    public class Circuit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Checkpoint> Checkpoints { get; set; }

        public SeasonEvent SeasonEvent { get; set; }

        // TODO: point calculation rules, bonuses, etc.
    }
}
