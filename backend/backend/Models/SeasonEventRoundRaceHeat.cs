namespace backend.Models
{
    public class SeasonEventRoundRaceHeat
    {
        public Guid Id { get; set; }

        public Guid RaceId { get; set; }
        public SeasonEventRoundRace Race { get; set; }

        public List<SeasonEventRoundRaceHeatResult> Results { get; set; }
    }
}
