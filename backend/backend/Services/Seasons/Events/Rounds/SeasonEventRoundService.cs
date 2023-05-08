using backend.Exceptions;
using backend.Models;
using backend.Models.Cars;
using backend.Models.Dtos.Seasons.Events.Rounds;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats.HeatResults;
using backend.Models.Seasons.Events;
using backend.Models.Seasons.Events.Rounds;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events.Rounds.Races.Heats.HeatResults;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Seasons.Events.Rounds
{
    public class SeasonEventRoundService
    {
        private readonly BackendContext _context;

        public SeasonEventRoundService(BackendContext context)
        {
            _context = context;
        }

        public List<SeasonEventRoundDto> GetSeasonEventRounds(Guid seasonEventId)
        {
            return _context.SeasonEventRounds.Where(round => round.SeasonEventId == seasonEventId)
                .Include(round => round.Participants)
                .Include(round => round.Races).ThenInclude(race => race.Results).ThenInclude(r => r.Car)
                .Include(round => round.Races).ThenInclude(race => race.Heats).ThenInclude(h => h.Results).ThenInclude(r => r.Car)
                .Select(round => new SeasonEventRoundDto(round))
                .ToList();
        }

        public SeasonEventRoundDto GetRound(Guid seasonEventId, Guid roundId)
        {
            var round = _context.SeasonEventRounds.Where(round => (round.SeasonEventId == seasonEventId) && (round.Id == roundId))
                .Include(round => round.Participants)
                .Include(round => round.Races).ThenInclude(race => race.Results).ThenInclude(r => r.Car)
                .FirstOrDefault();

            if (round == null)
            {
                throw new NotFoundException($"Round [{roundId}] not found in Season Event [{seasonEventId}]");
            }

            return new SeasonEventRoundDto(round);
        }

        public async Task<SeasonEventRoundDto> AddSeasonEventRound(Guid seasonEventId, SeasonEventRoundDto roundDto)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);
            var round = roundDto.ToEntity();

            if (seasonEvent.Rounds == null)
            {
                seasonEvent.Rounds = new List<SeasonEventRound>();
            }

            if (round.Order == 0)
            {
                round.Participants = seasonEvent.Participants;
            }

            round.SeasonEvent = seasonEvent;
            await _context.SeasonEventRounds.AddAsync(round);

            seasonEvent.Rounds.Add(round);

            await _context.SaveChangesAsync();

            return new SeasonEventRoundDto(round);
        }

        public async Task<SeasonEventRoundDto> UpdateSeasonEventRound(Guid seasonEventId, SeasonEventRoundDto newRound)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);
            var existingRound = seasonEvent.Rounds.Where(r => r.Id == newRound.Id).FirstOrDefault();
            
            if (existingRound == null)
            {
                throw new NotFoundException($"Update Round [{newRound.Id}] failed: Round not found in Season Event [{seasonEventId}]");
            }

            // TODO: potential data loss:
            // changing/deleting existing round will result in loosing all results from that round!
            // warn on frontend and make rounds/races anti-cascade delete, e.g. "isDeleted" flag

            // NOTE that if races weren't included in the query, they woundn't be deleted
            // there is a new empty list of races assigned to entity, but if it wasn't included,
            // it would be null anyway and EF wouldn't know that it has to delete anything
            // Of course, just not deleting them lead to errors (adding more and more races, not replacing them)

            // need to do it explicitely anyways (to allow re-adding the same races with the same ids)
            _context.SeasonEventRoundRaces.RemoveRange(existingRound.Races);
            await _context.SaveChangesAsync();

            newRound.ToEntity(existingRound);
            _context.SeasonEventRoundRaces.AddRange(existingRound.Races);

            if (existingRound.Order == 0)
            {
                existingRound.Participants = seasonEvent.Participants;
            }

            await _context.SaveChangesAsync();

            return new SeasonEventRoundDto(existingRound);
        }

        public async Task<SeasonEventRoundDto> DeleteSeasonEventRound(Guid seasonEventId, Guid roundId)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);
            var round = seasonEvent.Rounds.FirstOrDefault(r => r.Id == roundId);

            if (round == null)
            {
                throw new NotFoundException($"Failed to delete Season Event Round [{roundId}]: Round not found in Season Event [{seasonEvent.Name}]");
            }

            _context.SeasonEventRounds.Remove(round);
            await _context.SaveChangesAsync();

            return new SeasonEventRoundDto(round);
        }

        public async Task<SeasonEventRoundDto> DrawRaces(Guid roundId)
        {
            var round = await _context.SeasonEventRounds
                .Where(r => r.Id == roundId)
                .Include(r => r.Participants)
                .Include(r => r.Races).ThenInclude(r => r.Results).ThenInclude(r => r.Car)
                .Include(r => r.Races).ThenInclude(r => r.Heats).ThenInclude(h => h.Results).ThenInclude(r => r.Car)
                .Include(r => r.SecondChanceRules)
                .FirstOrDefaultAsync();
            
            if (round == null)
            {
                throw new NotFoundException($"Failed to draw Races of Event Round [{roundId}]: Round not found");
            }

            var random = new Random();
            var participants = new List<Car>(round.Participants);
            
            var races = round.Races.Select(r => new SeasonEventRoundRaceDto(r)).ToList();

            races.ForEach(race =>
            {
                var heatOrder = 0;

                while (race.ParticipantsCount > race.Results.Count)
                {
                    var participantIndex = random.Next(participants.Count);
                    var participant = participants[participantIndex];
                    participants.Remove(participant);

                    var raceResult = new SeasonEventRoundRaceResultDto();
                    raceResult.CarId = participant.Id;
                    raceResult.RaceId = race.Id;
                    raceResult.RaceOutcome = RaceOutcome.Undetermined;
                    raceResult.Position = 0;
                    raceResult.Points = 0;

                    race.Results.Add(raceResult);
                    _context.SeasonEventRoundRaceResults.Add(raceResult.ToEntity());

                    var heat = new SeasonEventRoundRaceHeatDto();

                    heat.Order = heatOrder;
                    heatOrder++;

                    var heatResults = new RaceHeatResultDto();
                    heatResults.CarId = participant.Id;

/*                    heatResults.SectorTimes = new float [0];
                    heatResults.FullTime = 0;

                    heatResults.TimePoints = 0;
                    heatResults.AdvantagePoints = 0;
*/                  heatResults.DistancePoints = 0;

                    heatResults.Bonuses = new float[0];

                    heatResults.Position = 0;
                    heatResults.PointsSummed = 0;

                    var sectorOneResults = new RaceHeatSectorResultDto();
                    sectorOneResults.Order = 0;
                    sectorOneResults.PositionPoints = 1;
                    sectorOneResults.AdvantagePoints = 3.50f;

                    var sectorTwoResults = new RaceHeatSectorResultDto();
                    sectorTwoResults.Order = 1;
                    sectorTwoResults.AdvantagePoints = 2.45f;

                    heatResults.SectorResults.AddRange(new List<RaceHeatSectorResultDto> {sectorOneResults, sectorTwoResults });

                    heat.RaceId = race.Id;
                    heat.Results.Add(heatResults);

                    _context.SeasonEventRoundRaceHeats.Add(heat.ToEntity());
                }
            });

            await _context.SaveChangesAsync();
            return new SeasonEventRoundDto(round);
        }

        public async Task<bool> HasRoundStarted(Guid roundId)
        {
            var races = await _context.SeasonEventRoundRaces
                .Where(r => r.RoundId == roundId)
                .Include(r => r.Heats).ThenInclude(h => h.Results)
                .ToListAsync();

            return races
                .SelectMany(r => r.Heats)
                .SelectMany(h => h.Results)
                .Any(result => result.PointsSummed != 0);
        }

        private async Task<SeasonEvent> GetSeasonEventOrThrow(Guid seasonEventId) 
        {
            return await GetSeasonEventOrThrow(seasonEventId, $"Season Event [{seasonEventId}] not found");
        }

        private async Task<SeasonEvent> GetSeasonEventOrThrow(Guid seasonEventId, string errorMessage) 
        {
            var seasonEvent = await _context
                .SeasonEvents
                .Where(seasonEvent => seasonEvent.Id == seasonEventId)
                .Include(seasonEvent => seasonEvent.Rounds).ThenInclude(r => r.Races)
                .Include(seasonEvent => seasonEvent.Rounds).ThenInclude(r => r.Participants)
                .Include(sE => sE.Participants)
                .FirstOrDefaultAsync();

            if (seasonEvent == null)
            {
                throw new NotFoundException(errorMessage);
            }

            return seasonEvent;
        }
    }
}
