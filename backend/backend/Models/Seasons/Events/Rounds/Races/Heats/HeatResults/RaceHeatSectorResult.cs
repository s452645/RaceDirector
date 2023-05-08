namespace backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class RaceHeatSectorResult
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public float Time { get; set; }
        public int Position { get; set; }
        public float PositionPoints { get; set; }
        public float AdvantagePoints { get; set; }

        public Guid RaceHeatResultId { get; set; }
        public RaceHeatResult RaceHeatResult { get; set; }

    }
}
