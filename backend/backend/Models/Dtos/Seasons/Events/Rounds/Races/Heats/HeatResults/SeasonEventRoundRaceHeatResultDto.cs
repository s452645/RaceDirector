using backend.Models.Dtos.Cars;
using backend.Models.Seasons.Events.Circuits;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public abstract class SeasonEventRoundRaceHeatResultDto
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public CarDto? Car { get; set; }
        public Guid HeatId { get; set; }

        public Track Track { get; set; }

        public float DistancePoints { get; set; }
        public float[] Bonuses { get; set; }

        public float PointsSummed { get; set; }
        public int Position { get; set; }

        public abstract void ProcessResultsChanges();
    }
}
