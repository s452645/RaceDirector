using backend.Models.Dtos.Cars;
using backend.Models.Seasons.Events.Circuits;
using backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class TimeTrialHeatResultDto : SeasonEventRoundRaceHeatResultDto
    {
        public float[] SectorTimes { get; set; }
        public float FullTime { get; set; }
        public float TimePoints { get; set; }


        public TimeTrialHeatResultDto()
        {
            Id = Guid.NewGuid();
            Track = Track.ALL;
            SectorTimes = new float[0];
            FullTime = 0;
            TimePoints = 0;
            DistancePoints = 0;
            Bonuses = new float[0];
            PointsSummed = 0;
            Position = 0;
        }

        public TimeTrialHeatResultDto(Guid id, Guid carId, Track track, float distancePoints, float[] bonuses, float pointsSummed, int position, Guid heatId, float[] sectorTimes, float fullTime, float timePoints)
        {
            Id = id;
            CarId = carId;
            Track = track;

            DistancePoints = distancePoints;
            Bonuses = bonuses;
            PointsSummed = pointsSummed;
            Position = position;
            HeatId = heatId;

            SectorTimes = sectorTimes;
            FullTime = fullTime;
            TimePoints = timePoints;
        }

        public TimeTrialHeatResultDto(TimeTrialHeatResult heatResult)
        {
            Id = heatResult.Id;

            CarId = heatResult.CarId;
            Car = new CarDto(heatResult.Car);

            HeatId = heatResult.HeatId;
            Track = heatResult.Track;

            SectorTimes = heatResult.SectorTimes;
            FullTime = heatResult.FullTime;
            TimePoints = heatResult.TimePoints;
            DistancePoints = heatResult.DistancePoints;
            Bonuses = heatResult.Bonuses;
            PointsSummed = heatResult.PointsSummed;
            Position = heatResult.Position;
        }

        public TimeTrialHeatResult ToEntity()
        {
            var result = new TimeTrialHeatResult();

            result.Id = Id;
            result.Track = Track;
            result.SectorTimes = SectorTimes;
            result.FullTime = FullTime;
            result.TimePoints = TimePoints;
            result.DistancePoints = DistancePoints;
            result.Bonuses = Bonuses;
            result.PointsSummed = PointsSummed;
            result.Position = Position;

            result.CarId = CarId;
            result.HeatId = HeatId;

            return result;
        }

        public override void ProcessResultsChanges()
        {
            throw new NotImplementedException();
        }
    }
}
