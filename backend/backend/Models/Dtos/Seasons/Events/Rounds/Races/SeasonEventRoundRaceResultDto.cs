using backend.Models.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events.Rounds;
using backend.Models.Cars;
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
        public RaceOutcome RaceOutcome { get; set; }

        public SeasonEventRoundRaceResultDto()
        {
            Id = Guid.NewGuid();
        }

        public SeasonEventRoundRaceResultDto(SeasonEventRoundRaceResult raceResult)
        {
            Id = raceResult.Id;
            CarId = raceResult.CarId;
            Car = new CarDto(raceResult.Car);
            RaceId = raceResult.SeasonEventRoundRaceId;
            Position = raceResult.Position;
            Points = raceResult.Points;
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

            return result;
        }
    }
}
