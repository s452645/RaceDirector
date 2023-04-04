using Microsoft.AspNetCore.Mvc;
using backend.Services.Seasons;
using backend.Models.Dtos.Seasons.Events;
using backend.Models.Dtos.Seasons;
using backend.Models.Seasons;

namespace backend.Controllers.Seasons
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly SeasonService _seasonService;

        public SeasonsController(SeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        // GET: api/Seasons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeasonDto>>> GetSeasons()
        {
            return await _seasonService.GetSeasons();
        }

        // GET: api/Seasons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SeasonDto>> GetSeason(Guid id)
        {
            return await _seasonService.GetSeason(id);
        }

        // POST: api/Seasons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Season>> PostSeason(SeasonDto season)
        {
            await _seasonService.AddSeason(season);
            return CreatedAtAction("GetSeason", new { id = season.Id }, season);
        }

        // DELETE: api/Seasons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeason(Guid id)
        {
            await _seasonService.DeleteSeason(id);
            return NoContent();
        }

        [HttpGet("{seasonId}/season-events")]
        public async Task<ActionResult<List<SeasonEventDto>>> GetSeasonEvents(Guid seasonId)
        {
            return await _seasonService.GetSeasonEvents(seasonId);
        }

        [HttpGet("{seasonId}/season-events/{seasonEventId}")]
        public async Task<ActionResult<SeasonEventDto>> GetSeasonEventById(Guid seasonId, Guid seasonEventId)
        {
            return await _seasonService.GetSeasonEventById(seasonId, seasonEventId);
        }

        [HttpPost("{seasonId}/season-events")]
        public async Task<ActionResult<SeasonEventDto>> AddSeasonEvent(Guid seasonId, SeasonEventDto seasonEvent)
        {
            var createdEvent = await _seasonService.AddSeasonEvent(seasonId, seasonEvent);
            return Ok(createdEvent);
        }

        [HttpDelete("{seasonId}/season-events/{seasonEventId}")]
        public async Task<ActionResult<SeasonEventDto>> DeleteSeasonEvent(Guid seasonId, Guid seasonEventId)
        {
            await _seasonService.DeleteSeasonEvent(seasonId, seasonEventId);
            return NoContent();
        }

        [HttpPost("{seasonEventId}/score-rules")]
        public async Task<ActionResult<SeasonEventDto>> AddScoreRules(Guid seasonEventId, SeasonEventScoreRulesDto scoreRulesDto)
        {
            var seasonEvent = await _seasonService.AddScoreRules(seasonEventId, scoreRulesDto);
            return Ok(seasonEvent);
        }
    }
}
