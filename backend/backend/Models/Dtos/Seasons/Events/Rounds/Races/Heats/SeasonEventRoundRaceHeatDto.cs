using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults;
using backend.Models.Seasons.Events.Rounds.Races.Heats;

namespace backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats
{
    public class SeasonEventRoundRaceHeatDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public HeatState State { get; set; }

        public Guid RaceId { get; set; }

        // TODO
        public List<RaceHeatResultDto> Results { get; set; }

        public SeasonEventRoundRaceHeatDto()
        {
            Id = Guid.NewGuid();
            Order = 0;
            State = HeatState.Inactive;
            Results = new List<RaceHeatResultDto>();
        }

        public SeasonEventRoundRaceHeatDto(Guid id, int order, HeatState state, Guid raceId, List<RaceHeatResultDto> results)
        {
            Id = id;
            Order = order;
            State = state;
            RaceId = raceId;
            Results = results;
        }

        public SeasonEventRoundRaceHeatDto(SeasonEventRoundRaceHeat raceHeat)
        {
            Id = raceHeat.Id;
            Order = raceHeat.Order;
            State = raceHeat.State;
            RaceId = raceHeat.Race.Id;
            Results = raceHeat.Results.Select(r => new RaceHeatResultDto(r)).ToList();
        }

        public SeasonEventRoundRaceHeat ToEntity()
        {
            var raceHeat = new SeasonEventRoundRaceHeat();
            raceHeat.Id = Id;
            raceHeat.Order = Order;
            raceHeat.State = State;
            raceHeat.RaceId = RaceId;
            raceHeat.Results = Results.Select(r => r.ToEntity()).ToList();

            return raceHeat;
        }


        // TODO: not generic at all
        public void processResultsChanges(SeasonEventScoreRulesDto scoreRules)
        {
            processSectorResultsChanges(0, new() { 5, 3, 2, 1 });
            processSectorResultsChanges(1, new() { 8, 5, 3, 1 });

            Results.ForEach(result => result.ProcessResultsChanges());

            Results.Sort((x, y) => x.PointsSummed.CompareTo(y.PointsSummed));

            if (scoreRules.TheMoreTheBetter)
            {
                Results.Reverse();
            }

            var position = 1;
            Results.ForEach(result =>
            {
                if (result.PointsSummed == 0)
                {
                    result.Position = 0;
                    return;
                }

                result.Position = position;
                position++;
            });
        }

        private void processSectorResultsChanges(int sectorOrder, List<int> pointsForPositions)
        {
            var sectorResults = Results.Select(r => r.SectorResults.Find(sectorResult => sectorResult.Order == sectorOrder))
                .Where(result => result is not null)
                .Select(result => result!)
                .ToList();

            if (sectorResults.Count == 0)
            {
                return;
            }

            sectorResults.Sort((x, y) => x.Time.CompareTo(y.Time));

            var advantagePoints = CalculateAdvantagePoints(sectorResults, pointsForPositions);

            int index = 0;

            sectorResults.ForEach(result =>
            {
                result.Position = index + 1;
                result.PositionPoints = pointsForPositions[index];
                result.AdvantagePoints = advantagePoints.GetRange(index, advantagePoints.Count - index).Sum();

                index++;
            });
        }

        private List<float> CalculateAdvantagePoints(List<RaceHeatSectorResultDto> sortedSectorResults, List<int> pointsForPositions)
        {
            float firstPlaceTime = sortedSectorResults[0].Time;
            float maxAdvantage = firstPlaceTime * 0.1f;

            float previousCarTime = firstPlaceTime;
            List<float> advantagesPoints = new();

            for (int i = 1; i < sortedSectorResults.Count; i++)
            {
                var currentCarTime = sortedSectorResults[i].Time;

                var advantage = Math.Min(currentCarTime - previousCarTime, maxAdvantage);
                var maxAdvantagePoints = pointsForPositions[i - 1] - pointsForPositions[i];

                var maxAdvantagePercent = advantage / maxAdvantage;
                var advantagePoints = maxAdvantagePercent * maxAdvantagePoints;

                advantagesPoints.Add(advantagePoints);
                previousCarTime = currentCarTime;
            }

            return advantagesPoints;
        }
    }
}
