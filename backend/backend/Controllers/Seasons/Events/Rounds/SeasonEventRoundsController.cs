using Microsoft.AspNetCore.Mvc;
using backend.Services.Seasons.Events.Rounds;
using backend.Models.Dtos.Seasons.Events.Rounds;

namespace backend.Controllers.Seasons.Events.Rounds
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonEventRoundsController : ControllerBase
    {
        private readonly SeasonEventRoundService _roundsService;

        public SeasonEventRoundsController(SeasonEventRoundService circuitService)
        {
            _roundsService = circuitService;
        }

        // GET: api/SeasonEventRounds
        [HttpGet()]
        public ActionResult<List<SeasonEventRoundDto>> GetRounds([FromQuery] Guid seasonEventId)
        {
            return Ok(_roundsService.GetSeasonEventRounds(seasonEventId));
        }

        // POST: api/SeasonEventRounds
        [HttpPost]
        public async Task<ActionResult<SeasonEventRoundDto>> PostRound([FromQuery] Guid seasonEventId, [FromBody] SeasonEventRoundDto roundDto)
        {
            return await _roundsService.AddSeasonEventRound(seasonEventId, roundDto);
        }

        // DELETE: api/SeasonEventRounds/5
        [HttpDelete("{roundId}")]
        public async Task<IActionResult> DeleteRound(Guid roundId, [FromQuery] Guid seasonEventId)
        {
            await _roundsService.DeleteSeasonEventRound(seasonEventId, roundId);
            return NoContent();
        }
    }
}
