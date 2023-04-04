namespace backend.Models
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

    public class SeasonEventRoundRaceResult
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public Guid SeasonEventRoundRaceId { get; set; }
        public SeasonEventRoundRace SeasonEventRoundRace { get; set; }

        public int? Position { get; set; }
        public float Points { get; set; }
        public RaceOutcome RaceOutcome { get; set; }
    }
}
