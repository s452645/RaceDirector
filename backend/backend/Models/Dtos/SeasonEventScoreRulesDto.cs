namespace backend.Models.Dtos
{
    public class SeasonEventScoreRulesDto
    {
        public Guid Id { get; set; }
        public float TimeMultipiler { get; set; }
        public float DistanceMultiplier { get; set; }
        public List<int> AvailableBonuses { get; set; }
        public float UnfinishedSectorPenaltyPoints { get; set; }
        public bool TheMoreTheBetter { get; set; }

        public Guid SeasonEventId { get; set; }

        public SeasonEventScoreRulesDto()
        {
            Id = Guid.NewGuid();
            AvailableBonuses = new List<int>();
        }

        public SeasonEventScoreRulesDto(
            Guid id, 
            float timeMultipiler, 
            float distanceMultiplier, 
            List<int> availableBonuses, 
            float unfinishedSectorPenaltyPoints, 
            bool theMoreTheBetter, 
            Guid seasonEventId
        )
        {
            Id = id;
            TimeMultipiler = timeMultipiler;
            DistanceMultiplier = distanceMultiplier;
            AvailableBonuses = availableBonuses;
            UnfinishedSectorPenaltyPoints = unfinishedSectorPenaltyPoints;
            TheMoreTheBetter = theMoreTheBetter;
            SeasonEventId = seasonEventId;
        }

        public SeasonEventScoreRulesDto(SeasonEventScoreRules scoreRules)
        {
            Id = scoreRules.Id;
            TimeMultipiler = scoreRules.TimeMultipiler;
            DistanceMultiplier = scoreRules.DistanceMultiplier;
            AvailableBonuses= scoreRules.AvailableBonuses.ToList();
            UnfinishedSectorPenaltyPoints = scoreRules.UnfinishedSectorPenaltyPoints;
            TheMoreTheBetter = scoreRules.TheMoreTheBetter;
            SeasonEventId = scoreRules.SeasonEvent.Id;
        }

        public SeasonEventScoreRules ToEntity()
        {
            var scoreRules = new SeasonEventScoreRules();
            scoreRules.Id = Id;
            scoreRules.TimeMultipiler = TimeMultipiler;
            scoreRules.DistanceMultiplier = DistanceMultiplier;
            scoreRules.AvailableBonuses = AvailableBonuses.ToArray();
            scoreRules.UnfinishedSectorPenaltyPoints = UnfinishedSectorPenaltyPoints;
            scoreRules.TheMoreTheBetter = TheMoreTheBetter;

            return scoreRules;
        }
    }

}
