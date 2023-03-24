﻿using backend.Models.Dtos;
using backenend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
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
                // TODO: throw not found exception
                throw new Exception();
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
                // TODO: throw not found exception
                throw new Exception();
            }

            _context.Seasons.Remove(season);
            await _context.SaveChangesAsync();

            return new SeasonDto(season);
        }

        public List<SeasonEventDto> GetSeasonEvents(Guid seasonId)
        {
            var season = _context.Seasons.Include(s => s.Events).ToList().Find(s => s.Id == seasonId);

            if (season == null)
            {
                // TODO: throw not found exception
                throw new Exception();
            }

            return season.Events.Select(e => new SeasonEventDto(e)).ToList();
        }

        public async Task<SeasonEventDto> AddSeasonEvent(Guid seasonId, SeasonEventDto seasonEvent)
        {
            var season = await _context.Seasons.FindAsync(seasonId);

            if (season == null) 
            {
                // TODO: throw not found exception
                throw new Exception();
            }

            var eventEntity = seasonEvent.ToEntity();
            eventEntity.Season = season;

            _context.SeasonEvents.Add(eventEntity);

            await _context.SaveChangesAsync();
            return seasonEvent;
        }

        public async Task<SeasonEventDto> DeleteSeasonEvent(Guid seasonId, Guid seasonEventId)
        {
            var season = _context.Seasons.Include(s => s.Events).ToList().Find(s => s.Id == seasonId);

            if (season == null)
            {
                // TODO: throw not found exception
                throw new Exception();
            }

            var seasonEvent = season.Events.Find(e => e.Id == seasonEventId);
            if (seasonEvent == null)
            {
                // TODO: throw not found exception
                throw new Exception();
            }

            _context.SeasonEvents.Remove(seasonEvent);
            await _context.SaveChangesAsync();

            return new SeasonEventDto(seasonEvent);
        }
    }
}