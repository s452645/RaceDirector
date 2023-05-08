using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons.Events.Circuits;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Services.Hardware;
using backend.Services.Hardware.Comms;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Threading.Tasks;
using System;

namespace backend.Services.Seasons.Events.Rounds.Races
{
    public class SeasonEventRoundRaceService
    {
        private readonly IServiceScopeFactory scopeFactory;

        private readonly BoardsManager _boardManager;
        private HeatManager? _heatManager;

        public SeasonEventRoundRaceService(IServiceScopeFactory scopeFactory, BoardsManager boardsManager)
        {
            this.scopeFactory = scopeFactory;
            _boardManager = boardsManager;
        }

        public SeasonEventRoundRaceDto GetRace(Guid roundId, Guid raceId)
        {
            using var scope = scopeFactory.CreateScope();
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
            using var scope = scopeFactory.CreateScope();
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

            public void BeginHeat(Guid heatId)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var heat = db.SeasonEventRoundRaceHeats
            .Where(h => h.Id == heatId)
            .Include(h => h.Results).ThenInclude(r => r.Car)
            .Include(h => h.Race).ThenInclude(r => r.Round).ThenInclude(r => r.SeasonEvent).ThenInclude(e => e.ScoreRules)
            .Include(h => h.Race).ThenInclude(r => r.Round).ThenInclude(r => r.SeasonEvent).ThenInclude(e => e.Circuit).ThenInclude(c => c.Checkpoints)
            .FirstOrDefault();

            if (heat == null)
            {
                throw new NotFoundException($"Cannot begin Heat: Heat [{heatId}] not found");
            }

            var heatDto = new SeasonEventRoundRaceHeatDto(heat);
            // FIXME
            var scoreRulesDto = new SeasonEventScoreRulesDto(heat.Race.Round.SeasonEvent.ScoreRules!);
            var circuitDto = new CircuitDto(heat.Race.Round.SeasonEvent.Circuit!);

            _heatManager = new HeatManager(scopeFactory, heatDto, circuitDto, scoreRulesDto);
            _boardManager.RegisterEventObserver(_heatManager);
        }

        public void RegisterHeatObserver(IHeatObserver heatObserver)
        {
            _heatManager?.Register(heatObserver);
        }

        public void UnregisterHeatObserver(IHeatObserver heatObserver)
        {
            _heatManager?.Unregister(heatObserver);
        }

        public void SaveDistanceAndBonuses(float distance, List<float> bonuses)
        {
            _heatManager?.ProcessDistanceAndBonuses(distance, bonuses);
        }

        public void EndHeat()
        {
            if (_heatManager != null)
            {
                _heatManager.EndHeat();
                _boardManager.RegisterEventObserver(_heatManager, true);
                _heatManager = null;
            }
        }
    }
}
