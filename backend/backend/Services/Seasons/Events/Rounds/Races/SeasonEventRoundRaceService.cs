using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using Microsoft.EntityFrameworkCore;
using backend.Controllers.Seasons.Events.Rounds.Races;

namespace backend.Services.Seasons.Events.Rounds.Races
{
    enum SeasonEventRoundRaceInclusion
    {
        ROUND_SEASONEVENT_CIRCUIT_CHECKPOINTS,
        ROUND_SEASONEVENT_SCORERULES,
    }

    public class SeasonEventRoundRaceService
    {
        private readonly RaceManager _raceManager;
        private readonly BackendContext _context;

        public SeasonEventRoundRaceService(BackendContext context, RaceManager raceManager)
        {
            _context = context;
            _raceManager = raceManager;
        }

        public SeasonEventRoundRaceDto GetRace(Guid roundId, Guid raceId)
        {
            var race = _context.SeasonEventRoundRaces
                .Where(r => (r.Id == raceId) && (r.Round.Id == roundId))
                .Include(r => r.Results).ThenInclude(r => r.Car)
                .Include(r => r.Round)
                .FirstOrDefault();

            if (race == null)
            {
                throw new NotFoundException($"Race [{raceId}] not found in round [{roundId}]");
            }

            return new SeasonEventRoundRaceDto(race);
        }

        public async Task<List<float>> GetRaceAvailableBonuses(Guid raceId)
        {
            var race = await _context.SeasonEventRoundRaces
                .Where(race => race.Id == raceId)
                .Include(race => race.Round).ThenInclude(round => round.SeasonEvent).ThenInclude(seasonEvent => seasonEvent.ScoreRules)
                .FirstOrDefaultAsync();

            if (race == null)
            {
                throw new NotFoundException($"Race [{raceId}] not found");
            }

            return race.Round.SeasonEvent.ScoreRules?.AvailableBonuses.ToList() ?? new();
        }

        public async Task SaveDistanceAndBonuses(Guid raceId, ResultData data)
        {
            await _raceManager.SaveDistanceAndBonuses(raceId, data);
        }

        public async Task StartRace(Guid raceId, Guid raceResultId)
        {
            await _raceManager.InitRace(raceId, raceResultId);
        }

        public async Task SetRaceDNF(Guid raceId, Guid raceResultId)
        {
            await _raceManager.setRaceDNF(raceId, raceResultId);
        }
    }
}