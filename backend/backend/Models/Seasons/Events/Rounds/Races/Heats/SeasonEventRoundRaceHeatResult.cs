using backend.Models.Cars;

namespace backend.Models.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeatResult
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public float[] SectorTimes { get; set; }
        public float FullTime { get; set; }

        public float TimePoints { get; set; }
        public float AdvantagePoints { get; set; }
        public float DistancePoints { get; set; }
        public float[] Bonuses { get; set; }

        public float PointsSummed { get; set; }
        public int Position { get; set; }
    }
}
