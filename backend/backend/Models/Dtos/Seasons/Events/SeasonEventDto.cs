using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Seasons.Events;
using backend.Models.Seasons.Events.Rounds;

namespace backend.Models.Dtos.Seasons.Events
{
    public class SeasonEventDto
    {

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SeasonEventType Type { get; set; }
        public SeasonEventScoreRulesDto? ScoreRules { get; set; }
        public CircuitDto? Circuit { get; set; }

        public int? ParticipantsCount { get; set; }

        public Guid? SeasonId { get; set; }

        public SeasonEventDto()
        {
            Id = Guid.NewGuid();
        }

        public SeasonEventDto(
            Guid id,
            string? name,
            DateTime startDate,
            DateTime endDate,
            SeasonEventType type,
            SeasonEventScoreRulesDto? scoreRules,
            CircuitDto? circuit,
            Guid? seasonId,
            int? participantsCount
        )
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            ScoreRules = scoreRules;
            Circuit = circuit;
            SeasonId = seasonId;
            ParticipantsCount = participantsCount;
        }

        public SeasonEventDto(SeasonEvent seasonEvent)
        {
            Id = seasonEvent.Id;
            Name = seasonEvent.Name;
            StartDate = seasonEvent.StartDate;
            EndDate = seasonEvent.EndDate;
            Type = seasonEvent.Type;

            if (seasonEvent.ScoreRules != null)
            {
                ScoreRules = new SeasonEventScoreRulesDto(seasonEvent.ScoreRules);
            }

            if (seasonEvent.Circuit != null)
            {
                Circuit = new CircuitDto(seasonEvent.Circuit);
            }

            SeasonId = seasonEvent.SeasonId;

            ParticipantsCount = seasonEvent.Participants?.Count;
        }

        public SeasonEvent ToEntity()
        {
            var seasonEvent = new SeasonEvent
            {
                Id = Id,
                Name = Name ?? string.Empty,
                StartDate = StartDate,
                EndDate = EndDate,
                Type = Type,

                Rounds = new List<SeasonEventRound>()
            };

            if (ScoreRules != null)
            {
                seasonEvent.ScoreRules = ScoreRules.ToEntity();
            }

            if (Circuit != null)
            {
                seasonEvent.Circuit = Circuit.ToEntity();
            }

            if (SeasonId != null)
            {
                seasonEvent.SeasonId = SeasonId.Value;
            }

            return seasonEvent;
        }
    }
}

