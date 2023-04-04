namespace backend.Models
{
    public class SeasonEventScoreRules
    {
        public Guid Id { get; set; }
        public float TimeMultiplier { get; set; }
        public float DistanceMultiplier { get; set; }
        public float[] AvailableBonuses { get; set; }
        public float UnfinishedSectorPenaltyPoints { get; set; }
        public bool TheMoreTheBetter { get; set; }

        public SeasonEvent SeasonEvent { get; set; }
    }
}
