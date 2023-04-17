using backend.Models.Seasons.Events.Rounds;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Cars;
using backend.Models.Dtos.Cars;

namespace backend.Models.Dtos.Seasons.Events.Rounds
{
    public class SeasonEventRoundDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int ParticipantsCount { get; set; }
        public RoundType Type { get; set; }

        public List<Guid> ParticipantsIds { get; set; }
        public List<SeasonEventRoundRaceDto> Races { get; set; }

        public RoundPointsStrategy PointsStrategy { get; set; }
        public DroppedCarsPositionDefinementStrategy DroppedCarsPositionDefinementStrategy { get; set; }
        public RoundPointsStrategy DroppedCarsPointsStrategy { get; set; }

        public int? AdvancesCount { get; set; }

        public Guid SeasonEventId { get; set; }

        public SeasonEventRoundDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            ParticipantsCount = 0;
            ParticipantsIds = new List<Guid>();
            Races = new List<SeasonEventRoundRaceDto>();
        }

        public SeasonEventRoundDto(
            Guid id,
            int order,
            int participantsCount,
            RoundType type,
            List<Guid> participantIds,
            List<SeasonEventRoundRaceDto> races,
            RoundPointsStrategy pointsStrategy,
            DroppedCarsPositionDefinementStrategy droppedCarsPositionDefinementStrategy,
            RoundPointsStrategy droppedCarsPointsStrategy,
            Guid seasonEventId,
            int? advancesCount
        )
        {
            Id = id;
            Order = order;
            ParticipantsCount = participantsCount;
            Type = type;
            ParticipantsIds = participantIds;
            Races = races;
            PointsStrategy = pointsStrategy;
            DroppedCarsPositionDefinementStrategy = droppedCarsPositionDefinementStrategy;
            DroppedCarsPointsStrategy = droppedCarsPointsStrategy;
            SeasonEventId = seasonEventId;
            AdvancesCount = advancesCount;
        }

        public SeasonEventRoundDto(SeasonEventRound seasonEventRound)
        {
            Id = seasonEventRound.Id;
            Order = seasonEventRound.Order;
            ParticipantsCount = seasonEventRound.ParticipantsCount;
            Type = seasonEventRound.Type;
            ParticipantsIds = seasonEventRound.Participants?.Select(p => p.Id).ToList() ?? new();
            Races = seasonEventRound.Races?.Select(race => new SeasonEventRoundRaceDto(race)).ToList() ?? new();
            PointsStrategy = seasonEventRound.PointsStrategy;
            DroppedCarsPositionDefinementStrategy = seasonEventRound.DroppedCarsPositionDefinementStrategy;
            DroppedCarsPointsStrategy = seasonEventRound.DroppedCarsPointsStrategy;
            SeasonEventId = seasonEventRound.SeasonEventId;

            int advancesCount = 0;
            seasonEventRound.Races.ForEach(r => advancesCount += r.InstantAdvancements);
            advancesCount += seasonEventRound.SecondChanceRules?.AdvancesCount ?? 0;

            AdvancesCount = advancesCount;
        }

        public SeasonEventRound ToEntity()
        {
            var round = new SeasonEventRound
            {
                Id = Id,
            };

            return ToEntity(round);
        }

        public SeasonEventRound ToEntity(SeasonEventRound entity)
        {
            entity.ParticipantsCount = ParticipantsCount;
            entity.Order = Order;
            entity.Type = Type;

            // dont map participants? make CarDto and then map?
            // remove participants Ids as they will be added later?
            // or maybe learn how to initialize lists in ef entities? xD
            entity.Participants = new List<Car>();

            entity.Races = Races.Select(race => race.ToEntity()).ToList();
            entity.PointsStrategy = PointsStrategy;
            entity.DroppedCarsPositionDefinementStrategy = DroppedCarsPositionDefinementStrategy;
            entity.DroppedCarsPointsStrategy = DroppedCarsPointsStrategy;

            return entity;
        }
    }
}
