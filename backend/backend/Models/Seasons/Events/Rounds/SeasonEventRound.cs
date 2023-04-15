using backend.Models.Cars;
using backend.Models.Seasons.Events.Rounds.Races;

namespace backend.Models.Seasons.Events.Rounds
{
    public enum RoundType
    {
        Ladder,
        Group,
        ClassicFinal,
        CandidateFinal,
    }

    public enum RoundPointsStrategy
    {
        OnlyThisRound,
        SummedWithPreviousRounds
    }

    public enum DroppedCarsPositionDefinementStrategy
    {
        RacePositionThenPoints,
        OnlyPoints
    }

    public class SeasonEventRound
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int ParticipantsCount { get; set; }
        public RoundType Type { get; set; }

        public List<Car> Participants { get; set; }
        public List<SeasonEventRoundRace> Races { get; set; }

        public Guid? SecondChanceRulesId { get; set; }
        public SecondChanceRules? SecondChanceRules { get; set; }

        public RoundPointsStrategy PointsStrategy { get; set; }
        public DroppedCarsPositionDefinementStrategy DroppedCarsPositionDefinementStrategy { get; set; }
        public RoundPointsStrategy DroppedCarsPointsStrategy { get; set; }

        public Guid SeasonEventId { get; set; }
        public SeasonEvent SeasonEvent { get; set; }
    }
}
