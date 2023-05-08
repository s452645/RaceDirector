using backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults
{
    public class RaceHeatSectorResultDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public float Time { get; set; }
        public int Position { get; set; }
        public float PositionPoints { get; set; }
        public float AdvantagePoints { get; set; }

        public Guid RaceHeatResultId { get; set; }

        public RaceHeatSectorResultDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            Time = 0;
            Position = 0;
            PositionPoints = 0;
            AdvantagePoints = 0;
            RaceHeatResultId = Guid.Empty;
        }

        public RaceHeatSectorResultDto(Guid id, int order, float time, int position, float positionPoints, float advantagePoints, Guid raceHeatResultId)
        {
            Id = id;
            Order = order;
            Time = time;
            Position = position;
            PositionPoints = positionPoints;
            AdvantagePoints = advantagePoints;
            RaceHeatResultId = raceHeatResultId;
        }

        public RaceHeatSectorResultDto(RaceHeatSectorResult sectorResult)
        {
            Id = sectorResult.Id;
            Order = sectorResult.Order;
            Time = sectorResult.Time;
            Position = sectorResult.Position;
            PositionPoints = sectorResult.PositionPoints;
            AdvantagePoints = sectorResult.AdvantagePoints;
            RaceHeatResultId = sectorResult.RaceHeatResultId;
        }

        public RaceHeatSectorResult ToEntity()
        {
            var entity = new RaceHeatSectorResult();

            entity.Id = Id;
            entity.Order = Order;
            entity.Time = Time;
            entity.Position = Position;
            entity.PositionPoints = PositionPoints;
            entity.AdvantagePoints = AdvantagePoints;
            entity.RaceHeatResultId = RaceHeatResultId;

            return entity;
        }
    }
}
