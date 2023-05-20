using backend.Models.Cars;
using backend.Models.Seasons.Events.Circuits;
using backend.Models.Seasons.Events.Rounds;

namespace backend.Models.Seasons.Events
{
    public enum SeasonEventType
    {
        Race,
        TimeTrial,
    }

    public class SeasonEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SeasonEventType Type { get; set; }

        public Guid? ScoreRulesId { get; set; }
        public SeasonEventScoreRules? ScoreRules { get; set; }

        public Guid? CircuitId { get; set; }
        public Circuit? Circuit { get; set; }

        public List<SeasonEventRound> Rounds { get; set; } = new List<SeasonEventRound>();
        public List<Car> Participants { get; set; } = new List<Car>();

        public Guid SeasonId { get; set; }
        public Season Season { get; set; }
    }
}
