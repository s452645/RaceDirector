using backend.Exceptions;
using backend.Models;
using backend.Models.Dtos.Seasons.Events.Rounds;
using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Models.Dtos.Seasons.Events.Rounds.Races.Heats;
using backend.Models.Seasons.Events;
using backend.Models.Seasons.Events.Rounds;
using backend.Models.Seasons.Events.Rounds.Races;
using backend.Models.Seasons.Events.Rounds.Races.Heats;
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
                .Include(round => round.Races).ThenInclude(race => race.Heats).ThenInclude(h => h.Results).ThenInclude(r => r.Car)
                .FirstOrDefault();

            if (round == null)
            {
                throw new NotFoundException($"Round [{roundId}] not found in Season Event [{seasonEventId}]");
            }

            return new SeasonEventRoundDto(round);
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
            var participants = round.Participants;
            
            var races = round.Races.Select(r => new SeasonEventRoundRaceDto(r)).ToList();

            races.ForEach(race =>
            {
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

                    var heatResults = new SeasonEventRoundRaceHeatResultDto();
                    heatResults.CarId = participant.Id;

                    heatResults.SectorTimes = new float [0];
                    heatResults.FullTime = 0;

                    heatResults.TimePoints = 0;
                    heatResults.AdvantagePoints = 0;
                    heatResults.DistancePoints = 0;

                    heatResults.Bonuses = new float[0];

                    heatResults.Position = 0;
                    heatResults.PointsSummed = 0;

                    heat.RaceId = race.Id;
                    heat.Results.Add(heatResults);

                    _context.SeasonEventRoundRaceHeats.Add(heat.ToEntity());
                }
            });

            await _context.SaveChangesAsync();
            return new SeasonEventRoundDto(round);
        }
    }
}
