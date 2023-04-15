using backend.Models.Cars;
using backend.Models.Dtos.Cars;
using backend.Models.Seasons.Events.Rounds.Races.Heats;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeatResultDto
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public CarDto? Car { get; set; }
        public Guid HeatId { get; set; }

        public float[] SectorTimes { get; set; }
        public float FullTime { get; set; }

        public float TimePoints { get; set; }
        public float AdvantagePoints { get; set; }
        public float DistancePoints { get; set; }
        public float[] Bonuses { get; set; }

        public float PointsSummed { get; set; }
        public int Position { get; set; }

        public SeasonEventRoundRaceHeatResultDto()
        {
            Id = Guid.NewGuid();
            SectorTimes = new float[0];
            FullTime = 0;
            TimePoints = 0;
            AdvantagePoints = 0;
            DistancePoints = 0;
            Bonuses = new float[0];
            PointsSummed = 0;
            Position = 0;
        }

        public SeasonEventRoundRaceHeatResultDto(SeasonEventRoundRaceHeatResult heatResult)
        {
            Id = heatResult.Id;

            CarId = heatResult.CarId;
            Car = new CarDto(heatResult.Car);

            HeatId = heatResult.HeatId;

            SectorTimes = heatResult.SectorTimes;
            FullTime = heatResult.FullTime;
            TimePoints = heatResult.TimePoints;
            AdvantagePoints = heatResult.AdvantagePoints;
            DistancePoints = heatResult.DistancePoints;
            Bonuses = heatResult.Bonuses;
            PointsSummed = heatResult.PointsSummed;
            Position = heatResult.Position;
        }

        public SeasonEventRoundRaceHeatResult ToEntity()
        {
            var result = new SeasonEventRoundRaceHeatResult();

            result.Id = Id;
            result.SectorTimes = SectorTimes;
            result.FullTime = FullTime;
            result.TimePoints = TimePoints;
            result.AdvantagePoints = AdvantagePoints;
            result.DistancePoints = DistancePoints;
            result.Bonuses = Bonuses;
            result.PointsSummed = PointsSummed;
            result.Position = Position;

            result.CarId = CarId;
            result.HeatId = HeatId;

            return result;
        }
    }
}
