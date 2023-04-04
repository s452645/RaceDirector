using backend.Models.Seasons.Events.Rounds.Races;

namespace backend.Models.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeat
    {
        public Guid Id { get; set; }

        public Guid RaceId { get; set; }
        public SeasonEventRoundRace Race { get; set; }

        public List<SeasonEventRoundRaceHeatResult> Results { get; set; }
    }
}
