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


        // POST: api/SeasonEventRoundRaces/6/begin-heat
        [HttpPost("{heatId}/begin-heat")]
        public IActionResult BeginHeat(Guid heatId)
        {
            _raceService.BeginHeat(heatId);
            return Ok();
        }

        // POST: api/SeasonEventRoundRaces/save-heat-data
        [HttpPost("save-heat-data")]
        public IActionResult SaveDistanceAndBonuses([FromBody] HeatData heatData) 
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
