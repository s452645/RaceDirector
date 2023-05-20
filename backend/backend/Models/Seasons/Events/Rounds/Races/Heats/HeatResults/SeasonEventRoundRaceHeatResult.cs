using backend.Models.Cars;
using backend.Models.Seasons.Events.Circuits;

namespace backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public abstract class SeasonEventRoundRaceHeatResult
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public Guid HeatId { get; set; }
        public SeasonEventRoundRaceHeat Heat { get; set; }

        public Track Track { get; set; }

        public float DistancePoints { get; set; }
        public float[] Bonuses { get; set; }

        public float PointsSummed { get; set; }
        public int Position { get; set; }
    }
}
