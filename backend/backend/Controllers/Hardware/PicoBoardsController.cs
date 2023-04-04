using Microsoft.AspNetCore.Mvc;
using backend.Models.Dtos.Hardware;
using backend.Services.Hardware;

namespace backend.Controllers.Hardware
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicoBoardsController
        : ControllerBase
    {
        private readonly PicoBoardsService _picoBoardsService;

        public PicoBoardsController(PicoBoardsService picoBoardsService)
        {
            _picoBoardsService = picoBoardsService;
        }

        // GET: api/PicoBoards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PicoBoardDto>>> GetPicoBoards()
        {
            return await _picoBoardsService.GetPicoBoards();
        }

        // POST: api/PicoBoard
        [HttpPost]
        public async Task<ActionResult<PicoBoardDto>> AddPicoBoard(PicoBoardDto picoBoardDto)
        {
            return await _picoBoardsService.AddPicoBoard(picoBoardDto);
        }

        // DELETE: api/PicoBoard/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicoBoardDto(Guid id)
        {
            await _picoBoardsService.DeletePicoBoard(id);
            return NoContent();
        }

        // GET: api/PicoBoards/5/sensors
        [HttpGet("{id}/sensors")]
        public async Task<ActionResult<IEnumerable<BreakBeamSensorDto>>> GetBoardBreakBeamSensors(Guid id)
        {
            return await _picoBoardsService.GetBoardBreakBeamSensors(id);
        }

        // POST: api/PicoBoard/5/sensors
        [HttpPost("{id}/sensors")]
        public async Task<ActionResult<PicoBoardDto>> AddBreakBeamSensor(Guid id, [FromBody] BreakBeamSensorDto sensorDto)
        {
            return await _picoBoardsService.AddBreakBeamSensor(id, sensorDto);
        }

        // DELETE: api/PicoBoard/5/sensors
        [HttpDelete("{boardId}/sensors/{sensorId}")]
        public async Task<ActionResult<PicoBoardDto>> RemoveBreakBeamSensor(Guid boardId, Guid sensorId)
        {
            return await _picoBoardsService.RemoveBreakBeamSensor(boardId, sensorId);
        }
    }
}
