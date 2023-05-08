namespace backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class TimeTrialHeatResult : SeasonEventRoundRaceHeatResult
    {
        public float[] SectorTimes { get; set; }
        public float FullTime { get; set; }
        public float TimePoints { get; set; }

    }
}
