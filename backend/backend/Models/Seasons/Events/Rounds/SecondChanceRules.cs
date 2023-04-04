namespace backend.Models.Seasons.Events.Rounds
{
    public class SecondChanceRules
    {
        public Guid Id { get; set; }
        public int AdvancesCount { get; set; }
        public RoundPointsStrategy PointsStrategy { get; set; }

        public SeasonEventRound Round { get; set; }
    }
}
