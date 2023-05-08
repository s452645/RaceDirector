using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults;
using backend.Models.Seasons.Events.Rounds.Races.Heats;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeatDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }

        public Guid RaceId { get; set; }

        // TODO
        public List<RaceHeatResultDto> Results { get; set; }

        public SeasonEventRoundRaceHeatDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            Results = new List<RaceHeatResultDto>();
        }

        public SeasonEventRoundRaceHeatDto(Guid id, int order, Guid raceId, List<RaceHeatResultDto> results)
        {
            Id = id;
            Order = order;
            RaceId = raceId;
            Results = results;
        }

        public SeasonEventRoundRaceHeatDto(SeasonEventRoundRaceHeat raceHeat)
        {
            Id = raceHeat.Id;
            Order = raceHeat.Order;
            RaceId = raceHeat.Race.Id;
            Results = raceHeat.Results.Select(r => new RaceHeatResultDto(r)).ToList();
        }

        public SeasonEventRoundRaceHeat ToEntity()
        {
            var raceHeat = new SeasonEventRoundRaceHeat();
            raceHeat.Id = Id;
            raceHeat.Order = Order;
            raceHeat.RaceId = RaceId;
            raceHeat.Results = Results.Select(r => r.ToEntity()).ToList();

            return raceHeat;
        }
    }
}
