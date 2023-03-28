using Microsoft.AspNetCore.Mvc;
using backend.Models.Dtos;
using backend.Services;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitsController : ControllerBase
    {
        private readonly CircuitService _circuitService;

        public CircuitsController(CircuitService circuitService)
        {
            _circuitService= circuitService;
        }

        // GET: api/Circuits
        [HttpGet()]
        public async Task<ActionResult<CircuitDto>> GetCircuitDto([FromQuery] Guid seasonEventId, [FromQuery] Guid circuitId)
        {
            return await _circuitService.GetCircuitById(seasonEventId, circuitId);
        }

        // POST: api/Circuits
        [HttpPost]
        public async Task<ActionResult<CircuitDto>> PostCircuitDto([FromQuery] Guid seasonEventId, [FromBody] CircuitDto circuitDto)
        {
            return await _circuitService.AddCircuit(seasonEventId, circuitDto);
        }

        // DELETE: api/Circuits/5
        [HttpDelete("{circuitId}")]
        public async Task<IActionResult> DeleteCircuitDto(Guid circuitId, [FromQuery] Guid seasonEventId)
        {
            await _circuitService.DeleteCircuit(seasonEventId, circuitId);
            return NoContent();
        }
    }
}
