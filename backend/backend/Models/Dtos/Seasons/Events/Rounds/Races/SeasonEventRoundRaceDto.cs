using backend.Models.Seasons.Events.Rounds.Races;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Runtime.ExceptionServices;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races
{
    public class SeasonEventRoundRaceDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int ParticipantsCount { get; set; }

        public List<SeasonEventRoundRaceResultDto> Results { get; set; }

        public SeasonEventRoundRaceResultDto? FirstResultFromNextRace { get; set; }
        public SeasonEventRoundRaceResultDto? SecondResultFromNextRace { get; set; }

        public int InstantAdvancements { get; set; }
        public int SecondChances { get; set; }
        public Guid RoundId { get; set; }

        public SeasonEventRoundRaceDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            ParticipantsCount = 0;
            InstantAdvancements = 0;
            SecondChances = 0;

            Results = new List<SeasonEventRoundRaceResultDto>();
        }

        public SeasonEventRoundRaceDto(
            Guid id,
            int order,
            int participantsCount,
            List<SeasonEventRoundRaceResultDto> results,
            int instantAdvancements, 
            int secondChances, 
            Guid roundId
        )
        {
            Id = id;
            Order = order;
            ParticipantsCount = participantsCount;
            Results = results;
            InstantAdvancements = instantAdvancements;
            SecondChances = secondChances;
            RoundId = roundId;
        }

        public SeasonEventRoundRaceDto(SeasonEventRoundRace race, SeasonEventRoundRace? nextRace)
        {
            Id = race.Id;
            Order = race.Order;
            ParticipantsCount = race.ParticipantsCount;
         
            // TODO: deal with not-included relationships better
            Results = race.Results?.Select(r => new SeasonEventRoundRaceResultDto(r)).ToList() ?? new();
            
            InstantAdvancements = race.InstantAdvancements;
            SecondChances = race.SecondChances;
            RoundId = race.Round?.Id ?? Guid.Empty;

            if (nextRace != null)
            {
                var firstResultFromNextRace = nextRace.Results.FirstOrDefault();

                if (firstResultFromNextRace != null)
                {
                    FirstResultFromNextRace = new SeasonEventRoundRaceResultDto(firstResultFromNextRace);
                    var secondResultFromNextRace = nextRace.Results.Where(r => r.CarId != firstResultFromNextRace.CarId).FirstOrDefault();
                    if (secondResultFromNextRace != null)
                    {
                        SecondResultFromNextRace = new SeasonEventRoundRaceResultDto(secondResultFromNextRace);
                    }
                }
            }
        }

        public SeasonEventRoundRace ToEntity()
        {
            var race = new SeasonEventRoundRace();
            race.Id = Id;
            race.Order = Order;
            race.ParticipantsCount = ParticipantsCount;
            race.InstantAdvancements = InstantAdvancements;
            race.SecondChances = SecondChances;
            race.Results = Results.Select(r => r.ToEntity()).ToList();

            return race;
        }
    }
}
