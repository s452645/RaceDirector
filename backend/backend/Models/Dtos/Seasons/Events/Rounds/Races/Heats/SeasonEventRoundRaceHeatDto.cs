using backend.Models.Seasons.Events.Rounds.Races.Heats;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeatDto
    {
        public Guid Id { get; set; }

        public Guid RaceId { get; set; }

        public List<SeasonEventRoundRaceHeatResultDto> Results { get; set; }

        public SeasonEventRoundRaceHeatDto()
        {
            Id = Guid.NewGuid();
            Results = new List<SeasonEventRoundRaceHeatResultDto>();
        }

        public SeasonEventRoundRaceHeatDto(Guid id, Guid raceId, List<SeasonEventRoundRaceHeatResultDto> results)
        {
            Id = id;
            RaceId = raceId;
            Results = results;
        }

        public SeasonEventRoundRaceHeatDto(SeasonEventRoundRaceHeat raceHeat)
        {
            Id = raceHeat.Id;
            RaceId = raceHeat.Race.Id;
            Results = raceHeat.Results.Select(r => new SeasonEventRoundRaceHeatResultDto(r)).ToList();
        }

        public SeasonEventRoundRaceHeat ToEntity()
        {
            var raceHeat = new SeasonEventRoundRaceHeat();
            raceHeat.Id = Id;
            raceHeat.RaceId = RaceId;
            raceHeat.Results = Results.Select(r => r.ToEntity()).ToList();

            return raceHeat;
        }
    }
}
