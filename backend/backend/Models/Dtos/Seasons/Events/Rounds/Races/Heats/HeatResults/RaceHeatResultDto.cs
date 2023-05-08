using backend.Models.Dtos.Cars;
using backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class RaceHeatResultDto : SeasonEventRoundRaceHeatResultDto
    {
        public List<RaceHeatSectorResultDto> SectorResults { get; set; }

        public RaceHeatResultDto()
        {
            Id = Guid.NewGuid();
            DistancePoints = 0;
            Bonuses = new float[0];
            PointsSummed = 0;
            Position = 0;

            SectorResults = new List<RaceHeatSectorResultDto>();
        }

        public RaceHeatResultDto(Guid id, Guid carId, float distancePoints, float[] bonuses, float pointsSummed, int position, Guid heatId, List<RaceHeatSectorResultDto> sectorResults)
        {
            Id = id;
            CarId = carId;
            DistancePoints = distancePoints;
            Bonuses = bonuses;
            PointsSummed = pointsSummed;
            Position = position;
            HeatId = heatId;

            SectorResults = sectorResults;
        }

        public RaceHeatResultDto(RaceHeatResult raceHeatResult)
        {
            Id = raceHeatResult.Id;
            CarId = raceHeatResult.CarId;
            Car = new CarDto(raceHeatResult.Car);
            DistancePoints = raceHeatResult.DistancePoints;
            Bonuses = raceHeatResult.Bonuses;
            PointsSummed = raceHeatResult.PointsSummed;
            Position = raceHeatResult.Position;
            HeatId = raceHeatResult.HeatId;

            SectorResults = raceHeatResult.SectorResults?.Select(sectorResult => new RaceHeatSectorResultDto(sectorResult)).ToList() ?? new();
        }

        public RaceHeatResult ToEntity()
        {
            var entity = new RaceHeatResult();

            entity.Id = Id;
            entity.CarId = CarId;
            entity.DistancePoints = DistancePoints;
            entity.Bonuses = Bonuses;
            entity.PointsSummed = PointsSummed;
            entity.Position = Position;
            entity.HeatId = HeatId;

            entity.SectorResults = SectorResults.Select(sectorResult => sectorResult.ToEntity()).ToList();

            return entity;
        }

    }
}
