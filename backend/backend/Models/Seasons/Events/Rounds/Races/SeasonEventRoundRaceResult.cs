using backend.Models.Cars;

namespace backend.Models.Seasons.Events.Rounds.Races
{
    public enum RaceOutcome
    {
        Undetermined,
        Advanced,
        Dropped,

        SecondChanceUndetermined,
        SecondChanceAdvanced,
        SecondChanceDropped,
    }

    public enum RaceState
    {
        Inactive,
        Pending,
        Active,
        Paused,
    }

    public class SeasonEventRoundRaceResult
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public Guid SeasonEventRoundRaceId { get; set; }
        public SeasonEventRoundRace SeasonEventRoundRace { get; set; }

        public int? Position { get; set; }

        public float Points { get; set; }
        
        public float[] SectorTimes { get; set; }

        public long[] SectorTimestamps { get; set; }
        
        public float Distance { get; set; }
        
        public float BonusesPoints { get; set; }
     
        public RaceState RaceState { get; set; }

        public RaceOutcome RaceOutcome { get; set; }
    }
}
