using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Services.Seasons.Events.Rounds.Races;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Seasons.Events.Rounds.Races
{
    public class HeatData {
        public float Distance { get; set; }
        public List<float> Bonuses { get; set; } = new();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SeasonEventRoundRacesController : Controller
    {
        private readonly SeasonEventRoundRaceService _raceService;

        public SeasonEventRoundRacesController(SeasonEventRoundRaceService raceService)
        {
            _raceService = raceService;
        }

        // GET: api/SeasonEventRoundRaces/5
        [HttpGet("{id}")]
        public ActionResult<SeasonEventRoundRaceDto> GetRace(Guid id, [FromQuery] Guid roundId)
        {
            return Ok(_raceService.GetRace(roundId, id));
        }

        // GET: api/SeasonEventRoundRaces/5/bonuses
        [HttpGet("{id}/bonuses")]
        public async Task<ActionResult<List<float>>> GetRaceAvailableBonuses(Guid id)
        {
            return Ok(await _raceService.GetRaceAvailableBonuses(id));
        }


        // POST: api/SeasonEventRoundRaces/6/begin-heat
        [HttpPost("{heatId}/init-heat")]
        public async Task<IActionResult> InitHeat(Guid heatId)
        {
            await _raceService.InitHeat(heatId);
            return Ok();
        }

        // POST: api/SeasonEventRoundRaces/save-heat-data
        [HttpPost("{heatId}/{heatResultId}/save-heat-data")]
        public IActionResult SaveDistanceAndBonuses(Guid heatId, Guid heatResultId, [FromBody] HeatData heatData) 
        {
            _raceService.SaveDistanceAndBonuses(heatData.Distance, heatData.Bonuses);
            return Ok();
        }

        // POST: api/SeasonEventRoundRaces/save-heat-data
        [HttpPost("end-heat")]
        public IActionResult EndHeat()
        {
            _raceService.EndHeat();
            return NoContent();
        }
    }
}
