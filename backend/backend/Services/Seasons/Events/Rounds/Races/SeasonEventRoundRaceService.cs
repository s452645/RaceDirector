using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Services.Hardware;
using Microsoft.EntityFrameworkCore;
using backend.Services.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events;
using backend.Models.Seasons.Events.Rounds.Races.Heats;
using backend.Repositories;
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
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly BoardsManager _boardManager;
        private HeatManager? _heatManager;

        public SeasonEventRoundRaceService(IServiceScopeFactory scopeFactory, BoardsManager boardsManager)
        {
            _scopeFactory = scopeFactory;
            _boardManager = boardsManager;
        }

        public SeasonEventRoundRaceDto GetRace(Guid roundId, Guid raceId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var race = db.SeasonEventRoundRaces
                .Where(r => (r.Id == raceId) && (r.Round.Id == roundId))
                .Include(r => r.Results).ThenInclude(r => r.Car)
                .Include(r => r.Round)
                .Include(r => r.Heats).ThenInclude(h => h.Results).ThenInclude(h => h.Car)
                .Include(r => r.Heats).ThenInclude(h => h.Results).ThenInclude(r => r.SectorResults)
                .FirstOrDefault();

            if (race == null)
            {
                throw new NotFoundException($"Race [{raceId}] not found in round [{roundId}]");
            }

            return new SeasonEventRoundRaceDto(race);
        }

        public async Task<List<float>> GetRaceAvailableBonuses(Guid raceId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var race = await db.SeasonEventRoundRaces
                .Where(race => race.Id == raceId)
                .Include(race => race.Round).ThenInclude(round => round.SeasonEvent).ThenInclude(seasonEvent => seasonEvent.ScoreRules)
                .FirstOrDefaultAsync();

            if (race == null)
            {
                throw new NotFoundException($"Race [{raceId}] not found");
            }

            return race.Round.SeasonEvent.ScoreRules?.AvailableBonuses.ToList() ?? new();
        }

        public async Task InitHeat(Guid heatId)
        {
            if (_heatManager != null)
            {
                throw new BadRequestException("Init heat failed: there is an initialized heat already");
            }

            _heatManager = await InitHeatManager(heatId);
            _boardManager.RegisterEventObserver(_heatManager);
        }

        public async Task MoveHeatToPendingState(Guid heatId)
        {
            if (_heatManager == null)
            {
                throw new BadRequestException("Moving heat to pending state failed: no heat initialized");
            }

            if (_heatManager.Heat.Id != heatId)
            {
                throw new BadRequestException("Moving heat to pending state failed: mismatched heat ids");
            }

            await _heatManager.MoveToPendingState();
        }
        
        public void EndHeat(Guid heatId)
        {
            if (_heatManager != null && _heatManager.Heat.Id == heatId)
            {
                _heatManager.MoveToInactiveState();
                _boardManager.RegisterEventObserver(_heatManager, true);
                _heatManager = null;
            }
        }

        public async void UpdateHeatData(Guid heatId, Guid heatResultId, HeatData data)
        {
            if (_heatManager == null)
            {
                throw new BadRequestException("Updating heat data failed: no heat initialized");
            }

            if (_heatManager.Heat.Id != heatId)
            {
                throw new BadRequestException("Updating heat data failed: mismatched heat ids");
            }

            await _heatManager.UpdateBonuses(heatResultId, data.Bonuses);
            await _heatManager.UpdateDistance(heatResultId, data.Distance);
        }

        public void RegisterHeatObserver(IHeatObserver heatObserver)
        {
            _heatManager?.Register(heatObserver);
        }

        public void UnregisterHeatObserver(IHeatObserver heatObserver)
        {
            _heatManager?.Unregister(heatObserver);
        }

        private BackendContext GetDbContext()
        {
            using var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<BackendContext>();
        }

        private async Task<HeatManager> InitHeatManager(Guid heatId)
        {
            var heat = new SeasonEventRoundRaceHeatDto(await GetHeatOrThrow(heatId));

            var race = await GetRaceOrThrow(
                heat.RaceId,
                new() {
                    SeasonEventRoundRaceInclusion.ROUND_SEASONEVENT_CIRCUIT_CHECKPOINTS,
                    SeasonEventRoundRaceInclusion.ROUND_SEASONEVENT_SCORERULES
                });

            var circuit = GetCircuitDtoOrThrow(race);
            var scoreRules = GetScoreRulesDtoOrThrow(race);
            var seasonEventType = race.Round.SeasonEvent.Type;

            if (SeasonEventType.Race == seasonEventType)
            {
                var dbContext = GetDbContext();

                var heatRepository = new SeasonEventRoundRaceHeatRepository(dbContext);
                var syncBoardRepository = new SyncBoardResultRepository(dbContext);

                return new RaceHeatManager(heatRepository, syncBoardRepository, heat, circuit, scoreRules);
            }

            // return new TimeTrialHeatManager(_scopeFactory, heat, circuit, scoreRules);
            throw new CustomHttpException("Initializing HeatManager failed: unknown heat type");
        }

        private CircuitDto GetCircuitDtoOrThrow(SeasonEventRoundRace race)
        {
            var circuit = race.Round.SeasonEvent.Circuit;
            
            if (circuit is null)
            {
                throw new CustomHttpException("Circuit not found");
            }

            return new CircuitDto(circuit);
        }

        private SeasonEventScoreRulesDto GetScoreRulesDtoOrThrow(SeasonEventRoundRace race)
        {
            var scoreRules = race.Round.SeasonEvent.ScoreRules;

            if (scoreRules is null)
            {
                throw new CustomHttpException("Score rules not found");
            }

            return new SeasonEventScoreRulesDto(scoreRules);
        }

        private async Task<SeasonEventRoundRaceHeat> GetHeatOrThrow(Guid heatId)
        {
            var heat = await GetDbContext().SeasonEventRoundRaceHeats
                .Where(h => h.Id == heatId)
                .Include(h => h.Results)
                .FirstOrDefaultAsync();

            if (heat is null)
            {
                throw new NotFoundException($"Heat [{heatId}] not found");
            }

            return heat;
        }

        private async Task<SeasonEventRoundRace> GetRaceOrThrow(
            Guid raceId,
            List<SeasonEventRoundRaceInclusion> inclusions
        )
        {
            return await GetRaceOrThrow(raceId, inclusions, $"Race [{raceId}] not found");
        }

        private async Task<SeasonEventRoundRace> GetRaceOrThrow(
            Guid raceId, 
            List<SeasonEventRoundRaceInclusion> inclusions,
            string errorMessage
        )
        {
            var query = GetDbContext().SeasonEventRoundRaces.Where(race => race.Id == raceId);

            if (inclusions.Contains(SeasonEventRoundRaceInclusion.ROUND_SEASONEVENT_CIRCUIT_CHECKPOINTS))
            {
                query = IncludeRoundSeasonEventCircuitCheckpoints(query);
            }

            if (inclusions.Contains(SeasonEventRoundRaceInclusion.ROUND_SEASONEVENT_SCORERULES))
            {
                query = IncludeRoundSeasonEventScoreRules(query);
            }

            var race = await query.FirstOrDefaultAsync();

            if (race is null)
            {
                throw new NotFoundException(errorMessage);
            }

            return race;
        }

        private IQueryable<SeasonEventRoundRace> IncludeRoundSeasonEventCircuitCheckpoints(IQueryable<SeasonEventRoundRace> query)
        {
            return query
                .Include(race => race.Round)
                .ThenInclude(round => round.SeasonEvent)
                .ThenInclude(seasonEvent => seasonEvent.Circuit)
                # nullable disable
                // EF Core will not cause any errors if circuit is null
                .ThenInclude(circuit => circuit.Checkpoints);
                # nullable enable
        }

        private IQueryable<SeasonEventRoundRace> IncludeRoundSeasonEventScoreRules(IQueryable<SeasonEventRoundRace> query)
        {
            return query
                .Include(race => race.Round)
                .ThenInclude(round => round.SeasonEvent)
                .ThenInclude(seasonEvent => seasonEvent.ScoreRules);
        }
    }
}
