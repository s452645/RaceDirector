using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events.Rounds;
using backend.Models.Seasons.Events.Rounds;
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
                .Include(round => round.Races).ThenInclude(race => race.Results)
                .Select(round => new SeasonEventRoundDto(round))
                .ToList();
        }

        public async Task<SeasonEventRoundDto> AddSeasonEventRound(Guid seasonEventId, SeasonEventRoundDto roundDto)
        {
            var seasonEvent = await _context
                .SeasonEvents
                .Where(seasonEvent => seasonEvent.Id == seasonEventId)
                .Include(sE => sE.Participants)
                .FirstOrDefaultAsync();

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Failed to add new Season Event Round: Season Event [{seasonEventId}] not found");
            }

            var round = roundDto.ToEntity();

            if (seasonEvent.Rounds == null)
            {
                seasonEvent.Rounds = new List<SeasonEventRound>();
            }

            round.SeasonEvent = seasonEvent;
            await _context.SeasonEventRounds.AddAsync(round);

            seasonEvent.Rounds.Add(round);

            await _context.SaveChangesAsync();

            return new SeasonEventRoundDto(round);
        }

        public async Task<SeasonEventRoundDto> DeleteSeasonEventRound(Guid seasonEventId, Guid roundId)
        {
            var seasonEvent = await _context
                .SeasonEvents
                .Where(seasonEvent => seasonEvent.Id == seasonEventId)
                .Include(seasonEvent => seasonEvent.Rounds)
                .Include(sE => sE.Participants)
                .FirstOrDefaultAsync();

            if (seasonEvent == null)
            {
                throw new NotFoundException($"Failed to delete Season Event Round [{roundId}]: Season Event [{seasonEventId}] not found");
            }

            var round = seasonEvent.Rounds.FirstOrDefault(r => r.Id == roundId);

            if (round == null)
            {
                throw new NotFoundException($"Failed to delete Season Event Round [{roundId}]: Round not found in Season Event [{seasonEvent.Name}]");
            }

            _context.SeasonEventRounds.Remove(round);
            await _context.SaveChangesAsync();

            return new SeasonEventRoundDto(round);
        }
    }
}
