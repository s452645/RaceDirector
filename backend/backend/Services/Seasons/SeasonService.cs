using backend.Exceptions;
using backend.Models;
using backend.Models.Cars;
using backend.Models.Dtos.Seasons;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Seasons.Events;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Seasons
{
    public class SeasonService
    {
        private readonly BackendContext _context;

        public SeasonService(BackendContext context)
        {
            _context = context;
        }

        public async Task<List<SeasonDto>> GetSeasons()
        {
            var seasons = await _context.Seasons.ToListAsync();
            return seasons.Select(s => new SeasonDto(s)).ToList();
        }

        public async Task<SeasonDto> GetSeason(Guid id)
        {
            var season = await _context.Seasons.FindAsync(id);

            if (season == null)
            {
                throw new NotFoundException($"Season [{id}] not found");
            }

            return new SeasonDto(season);
        }

        public async Task<SeasonDto> AddSeason(SeasonDto season)
        {
            var entity = season.ToEntity();
            _context.Seasons.Add(entity);
            await _context.SaveChangesAsync();

            return new SeasonDto(entity);
        }

        public async Task<SeasonDto> DeleteSeason(Guid id)
        {
            var season = await _context.Seasons.FindAsync(id);

            if (season == null)
            {
                throw new NotFoundException($"Season [{id}] not found");
            }

            _context.Seasons.Remove(season);
            await _context.SaveChangesAsync();

            return new SeasonDto(season);
        }

        public async Task<List<SeasonEventDto>> GetSeasonEvents(Guid seasonId)
        {
            var season = await _context.Seasons
                .Include(s => s.Events)
                .FirstOrDefaultAsync(s => s.Id == seasonId);

            if (season == null)
            {
                throw new NotFoundException($"Season [{seasonId}] not found");
            }

            return season.Events.Select(e => new SeasonEventDto(e)).ToList();
        }

        public async Task<SeasonEventDto> GetSeasonEventById(Guid seasonId, Guid seasonEventId)
        {
            var season = await _context.Seasons
                .Include(s => s.Events).ThenInclude(e => e.Participants)
                .Include(s => s.Events).ThenInclude(e => e.ScoreRules)
                .Include(s => s.Events).ThenInclude(e => e.Circuit!).ThenInclude(c => c.Checkpoints).ThenInclude(c => c.BreakBeamSensor)
                .FirstOrDefaultAsync(s => s.Id == seasonId);

            if (season == null)
            {
                throw new NotFoundException($"Season [{seasonId}] not found");
            }

            var seasonEvent = season.Events.Find(e => e.Id == seasonEventId);

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Season event [{seasonEventId}] not found in season [{seasonId}]");
            }

            return new SeasonEventDto(seasonEvent);
        }

        public async Task<SeasonEventDto> AddSeasonEvent(Guid seasonId, SeasonEventDto seasonEvent)
        {
            var season = await _context.Seasons.FindAsync(seasonId);

            if (season == null)
            {
                throw new NotFoundException($"Season [{seasonId}] not found");
            }

            var eventEntity = seasonEvent.ToEntity();
            eventEntity.Season = season;
            eventEntity.Participants = _context.Cars.Where(c => true).ToList();

            _context.SeasonEvents.Add(eventEntity);

            await _context.SaveChangesAsync();
            return seasonEvent;
        }

        public async Task<SeasonEventDto> UpdateSeasonEvent(Guid seasonId, SeasonEventDto newEventDto)
        {
            var season = await _context.Seasons.Include(s => s.Events).FirstOrDefaultAsync(s => s.Id == seasonId);
            if (season == null)
            {
                throw new NotFoundException($"Season [{seasonId}] not found");
            }

            var existingSeasonEvent = season.Events.Find(e => e.Id == newEventDto.Id);
            if (existingSeasonEvent == null)
            {
                throw new NotFoundException($"Season event [{newEventDto.Id}] not found in season [{seasonId}]");
            }

            newEventDto.ToEntity(existingSeasonEvent);
            _context.SeasonEvents.Update(existingSeasonEvent);
            await _context.SaveChangesAsync();

            return new SeasonEventDto(existingSeasonEvent);
        }

        public async Task<SeasonEventDto> DeleteSeasonEvent(Guid seasonId, Guid seasonEventId)
        {
            var season = _context.Seasons.Include(s => s.Events).ToList().Find(s => s.Id == seasonId);

            if (season == null)
            {
                throw new NotFoundException($"Season [{seasonId}] not found");
            }

            var seasonEvent = season.Events.Find(e => e.Id == seasonEventId);
            if (seasonEvent == null)
            {
                throw new NotFoundException($"Season event [{seasonEventId}] not found in season [{seasonId}]");
            }

            _context.SeasonEvents.Remove(seasonEvent);
            await _context.SaveChangesAsync();

            return new SeasonEventDto(seasonEvent);
        }

        public async Task<SeasonEventDto> AddScoreRules(Guid seasonEventId, SeasonEventScoreRulesDto scoreRulesDto)
        {
            var seasonEvent = await GetSeasonEventOrThrow(seasonEventId);

            if (seasonEvent.ScoreRules != null)
            {
                _context.SeasonEventScoreRules.Remove(seasonEvent.ScoreRules);
            }

            var scoreRules = scoreRulesDto.ToEntity();

            _context.SeasonEventScoreRules.Add(scoreRules);
            seasonEvent.ScoreRules = scoreRules;
            await _context.SaveChangesAsync();

            return new SeasonEventDto(seasonEvent);
        }

        private async Task<SeasonEvent> GetSeasonEventOrThrow(Guid seasonEventId)
        {
            return await GetSeasonEventOrThrow(seasonEventId, $"Season Event [{seasonEventId}] not found");
        }

        private async Task<SeasonEvent> GetSeasonEventOrThrow(Guid seasonEventId, string errorMessage)
        {
            var seasonEvent = await _context.SeasonEvents.Include(se => se.ScoreRules).FirstOrDefaultAsync(se => se.Id == seasonEventId);

            if (seasonEvent == null)
            {
                throw new NotFoundException(errorMessage);
            }

            return seasonEvent;
        }
    }
}
