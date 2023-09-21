using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Cars;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races
{
    public class SeasonEventRoundRaceResultDto
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        public CarDto? Car { get; set; }

        public Guid RaceId { get; set; }

        public int? Position { get; set; }
        public float Points { get; set; }
        public List<float> SectorTimes { get; set; }
        public float Distance { get; set; }
        public float BonusesPoints { get; set; }

        public RaceState RaceState { get; set; }

        public RaceOutcome RaceOutcome { get; set; }

        public SeasonEventRoundRaceResultDto()
        {
            Id = Guid.NewGuid();
            SectorTimes = new();
        }

        public SeasonEventRoundRaceResultDto(SeasonEventRoundRaceResult raceResult)
        {
            Id = raceResult.Id;
            CarId = raceResult.CarId;
            Car = new CarDto(raceResult.Car);
            RaceId = raceResult.SeasonEventRoundRaceId;
            Position = raceResult.Position;
            Points = raceResult.Points;
            SectorTimes = raceResult.SectorTimes.ToList();
            Distance = raceResult.Distance;
            BonusesPoints = raceResult.BonusesPoints;
            RaceState = raceResult.RaceState;
            RaceOutcome = raceResult.RaceOutcome;
        }

        public SeasonEventRoundRaceResult ToEntity()
        {
            var result = new SeasonEventRoundRaceResult();
            result.Id = Id;
            result.CarId = CarId;
            result.SeasonEventRoundRaceId = RaceId;
            result.Position = Position;
            result.Points = Points;
            result.RaceOutcome = RaceOutcome;
            result.SectorTimes = SectorTimes.ToArray();
            result.BonusesPoints = BonusesPoints;
            result.Distance = Distance;
            result.RaceState = RaceState;

            return result;
        }
    }
}
