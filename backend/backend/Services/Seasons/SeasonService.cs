using backend.Exceptions;
using backend.Models;
using backend.Models.Cars;
using backend.Models.Dtos.Cars;
using backend.Models.Dtos.Seasons;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Owners;
using backend.Models.Seasons.Events;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

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

            // List<string> participantsIds = new List<string> { "Mj17BiitU0aoVNzlvtqwlC", "TLCLdUGxlrFaOXY7hLgEda", "TNYx3qz5eS46MftAKeNNrc", "vAUm9925klU7W2HMTeLIZz", "0yzJ3HYmPJphA7XDmK0Nsl", "9eMoGf6ej7VQ2eccABmhtm", "Y1fCZ2RfuroyEt18K8FbU7", "cbEoPtVPnZ4lQwyj_QmQf6", "S0W5qy05WC4de-snQL6aW4", "9theuRsOAq4Zii6PD2PKY1" };

            // var participants = _context.Cars
            //    .Where(c => participantsIds.Contains(c.Note))
            //    .ToList();

            // eventEntity.CircuitId = new Guid("6e377b07-6932-436d-8864-5aab7b389b8a");
            // eventEntity.Participants = participants;

            var placeholderCar1 = new Car();
            var placeholderCar2 = new Car();
            var placeholderCar3 = new Car();
            var placeholderCar4 = new Car();
            var placeholderCar5 = new Car();
            var placeholderCar6 = new Car();
            var placeholderCar7 = new Car();
            var placeholderCar8 = new Car();
            var placeholderCar9 = new Car();
            var placeholderCar10 = new Car();
            var placeholderCar11 = new Car();
            var placeholderCar12 = new Car();
            var placeholderCar13 = new Car();
            var placeholderCar14 = new Car();
            var placeholderCar15 = new Car();
            var placeholderCar16 = new Car();
            var placeholderCar17 = new Car();
            var placeholderCar18 = new Car();
            var placeholderCar19 = new Car();
            var placeholderCar20 = new Car();
            var placeholderCar21 = new Car();
            var placeholderCar22 = new Car();
            var placeholderCar23 = new Car();
            var placeholderCar24 = new Car();
            var placeholderCar25 = new Car();

            var placeholders = new List<Car> { placeholderCar1, placeholderCar2, placeholderCar3, placeholderCar4, placeholderCar5, placeholderCar6, placeholderCar7, placeholderCar8, placeholderCar9, placeholderCar10 };
            placeholders.ForEach(p =>
            {
                p.Name = "Placeholder";
            });

            _context.AddRange(placeholders);
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

        public async Task<List<CarDto>> GetSeasonEventParticipants(Guid seasonEventId)
        {
            var seasonEvent = await _context.SeasonEvents
                .Include(se => se.Participants)
                .Where(se => se.Id == seasonEventId)
                .FirstOrDefaultAsync();

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Cannot get participants: season event [{seasonEventId}] not found");
            }

            return seasonEvent.Participants.Select(p => new CarDto(p)).ToList();
        }

        public async Task<SeasonEventDto> AssignSeasonEventParticipants(Guid seasonEventId, List<Guid> participantsIds)
        {
            // TODO: block if started
            var seasonEvent = await _context.SeasonEvents
                .Include(se => se.Participants)
                .Where(se => se.Id == seasonEventId)
                .FirstOrDefaultAsync();

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Cannot assign participants to season event [{seasonEventId}]: season event not found");
            }

            var participants = await _context.Cars
                .Where(c => participantsIds.Contains(c.Id))
                .ToListAsync();

            if (participants.Count != participantsIds.Count)
            {
                throw new BadRequestException($"Cannot assign participants to season event [{seasonEventId}]. Some participants were not found.");
            }

            seasonEvent.Participants = participants;
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
