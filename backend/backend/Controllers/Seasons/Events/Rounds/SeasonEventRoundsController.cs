﻿using Microsoft.AspNetCore.Mvc;
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

        // GET: api/SeasonEventRounds/5
        [HttpGet("{id}")]
        public ActionResult<List<SeasonEventRoundDto>> GetRound(Guid id, [FromQuery] Guid seasonEventId)
        {
            return Ok(_roundsService.GetRound(seasonEventId, id));
        }

        // POST: api/SeasonEventRounds
        [HttpPost]
        public async Task<ActionResult<SeasonEventRoundDto>> PostRound([FromQuery] Guid seasonEventId, [FromBody] SeasonEventRoundDto roundDto)
        {
            return await _roundsService.AddSeasonEventRound(seasonEventId, roundDto);
        }

        // POST: api/SeasonEventRounds
        [HttpPost("{id}/draw")]
        public async Task<ActionResult<SeasonEventRoundDto>> DrawRaces(Guid id)
        {
            return await _roundsService.DrawRaces(id);
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
