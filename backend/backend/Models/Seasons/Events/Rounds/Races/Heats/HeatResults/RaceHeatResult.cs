namespace backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class RaceHeatResult : SeasonEventRoundRaceHeatResult
    {
        public List<RaceHeatSectorResult> SectorResults { get; set; }
    }
}
