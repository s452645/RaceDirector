using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults;

namespace backend.Models.Seasons.Events.Rounds.Races.Heats
{
    public enum HeatState
    {
        Inactive,
        Pending,
        Active,
    }

    public class SeasonEventRoundRaceHeat
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public HeatState State { get; set; } = HeatState.Inactive;

        public Guid RaceId { get; set; }
        public SeasonEventRoundRace Race { get; set; }

        // TODO
        public List<RaceHeatResult> Results { get; set; } = new List<RaceHeatResult>();
    }
}
