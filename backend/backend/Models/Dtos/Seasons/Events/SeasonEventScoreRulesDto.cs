using backend.Models.Seasons.Events;

namespace backend.Models.Dtos.Seasons.Events
{
    public class SeasonEventScoreRulesDto
    {
        public Guid Id { get; set; }
        public float TimeMultiplier { get; set; }
        public float DistanceMultiplier { get; set; }
        public List<float> AvailableBonuses { get; set; }
        public float UnfinishedSectorPenaltyPoints { get; set; }
        public bool TheMoreTheBetter { get; set; }

        public Guid SeasonEventId { get; set; }

        public SeasonEventScoreRulesDto()
        {
            Id = Guid.NewGuid();
            AvailableBonuses = new List<float>();
        }

        public SeasonEventScoreRulesDto(
            Guid id,
            float timeMultiplier,
            float distanceMultiplier,
            List<float> availableBonuses,
            float unfinishedSectorPenaltyPoints,
            bool theMoreTheBetter,
            Guid seasonEventId
        )
        {
            Id = id;
            TimeMultiplier = timeMultiplier;
            DistanceMultiplier = distanceMultiplier;
            AvailableBonuses = availableBonuses;
            UnfinishedSectorPenaltyPoints = unfinishedSectorPenaltyPoints;
            TheMoreTheBetter = theMoreTheBetter;
            SeasonEventId = seasonEventId;
        }

        public SeasonEventScoreRulesDto(SeasonEventScoreRules scoreRules)
        {
            Id = scoreRules.Id;
            TimeMultiplier = scoreRules.TimeMultiplier;
            DistanceMultiplier = scoreRules.DistanceMultiplier;
            AvailableBonuses = scoreRules.AvailableBonuses.ToList();
            UnfinishedSectorPenaltyPoints = scoreRules.UnfinishedSectorPenaltyPoints;
            TheMoreTheBetter = scoreRules.TheMoreTheBetter;
            SeasonEventId = scoreRules.SeasonEvent.Id;
        }

        public SeasonEventScoreRules ToEntity()
        {
            var scoreRules = new SeasonEventScoreRules();
            scoreRules.Id = Id;
            scoreRules.TimeMultiplier = TimeMultiplier;
            scoreRules.DistanceMultiplier = DistanceMultiplier;
            scoreRules.AvailableBonuses = AvailableBonuses.ToArray();
            scoreRules.UnfinishedSectorPenaltyPoints = UnfinishedSectorPenaltyPoints;
            scoreRules.TheMoreTheBetter = TheMoreTheBetter;

            return scoreRules;
        }
    }

}
