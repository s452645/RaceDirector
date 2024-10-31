using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Services.Hardware.Comms;
using backend.Services.Seasons.Events.Rounds.Races;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Seasons.Events.Rounds.Races
{
    public class ResultData {
        public Guid RaceResultId { get; set; }
        public float Distance { get; set; }
        public float BonusesPoints { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SeasonEventRoundRacesController : Controller
    {
        private readonly SeasonEventRoundRaceService _raceService;
        private readonly RaceManager _raceManager;

        public SeasonEventRoundRacesController(SeasonEventRoundRaceService raceService, RaceManager raceManager)
        {
            _raceService = raceService;
            _raceManager = raceManager;
        }

        // GET: api/SeasonEventRoundRaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SeasonEventRoundRaceDto>> GetRace(Guid id, [FromQuery] Guid roundId)
        {
            var result = await _raceService.GetRace(roundId, id);
            return Ok(result);
        }

        // GET: api/SeasonEventRoundRaces/5/bonuses
        [HttpGet("{id}/bonuses")]
        public async Task<ActionResult<List<float>>> GetRaceAvailableBonuses(Guid id)
        {
            return Ok(await _raceService.GetRaceAvailableBonuses(id));
        }

        // POST: api/SeasonEventRoundRaces/5/3/result-data
        [HttpPost("{raceId}/race-data")]
        public async Task<IActionResult> SaveDistanceAndBonuses(Guid raceId, [FromBody] ResultData resultData)
        {
            await _raceService.SaveDistanceAndBonuses(raceId, resultData);
            return Ok();
        }

        [HttpPost("{raceId}/{raceResultId}/start")]
        public async Task<IActionResult> StartRace(Guid raceId, Guid raceResultId)
        {
            await _raceService.StartRace(raceId, raceResultId);
            return Ok();
        }

        [HttpPost("{raceId}/{raceResultId}/dnf")]
        public async Task<IActionResult> DnfRace(Guid raceId, Guid raceResultId)
        {
            await _raceService.SetRaceDNF(raceId, raceResultId);
            return Ok();
        }
    }
}