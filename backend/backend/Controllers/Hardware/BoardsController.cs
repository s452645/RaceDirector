using Microsoft.AspNetCore.Mvc;
using backend.Models.Dtos.Hardware;
using backend.Services.Hardware;
using backend.Models.Hardware;


namespace backend.Controllers.Hardware
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly BoardsService _boardsService;

        public BoardsController(BoardsService boardsService)
        {
            _boardsService = boardsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PicoBoardDto>>> GetPicoBoards()
        {
            return await _boardsService.GetPicoBoards();
        }

        [HttpPost]
        public async Task<ActionResult<PicoBoardDto>> AddPicoBoard(PicoBoardDto picoBoardDto)
        {
            return await _boardsService.AddPicoBoard(picoBoardDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicoBoardDto(Guid id)
        {
            await _boardsService.DeletePicoBoard(id);
            return NoContent();
        }

        [HttpPost("sync-all")]
        public async Task<List<SyncBoardResult>> SyncAllBoards()
        {
            return await _boardsService.SyncAllBoards();
        }

        [HttpPost("start-listening-all")]
        public async Task<IActionResult> StartListeningToAllBoards()
        {
            await _boardsService.StartListeningToAllBoards();
            return Ok();
        }

        // GET: api/PicoBoards/5/sensors
        [HttpGet("{id}/sensors")]
        public async Task<ActionResult<IEnumerable<BreakBeamSensorDto>>> GetBoardBreakBeamSensors(Guid id)
        {
            return await _boardsService.GetBoardBreakBeamSensors(id);
        }

        // POST: api/PicoBoard/5/sensors
        [HttpPost("{boardId}/sensors")]
        public async Task<ActionResult<PicoBoardDto>> AddBreakBeamSensor(Guid boardId, [FromBody] BreakBeamSensorDto sensorDto)
        {
            return await _boardsService.AddBreakBeamSensor(boardId, sensorDto);
        }

        // DELETE: api/PicoBoard/5/sensors
        [HttpDelete("{boardId}/sensors/{sensorId}")]
        public async Task<ActionResult<PicoBoardDto>> RemoveBreakBeamSensor(Guid boardId, Guid sensorId)
        {
            return await _boardsService.RemoveBreakBeamSensor(boardId, sensorId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("boards-websocket")]
        public async Task BoardsWebsocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                var observer = new BoardListenerObserver(webSocket, socketFinishedTcs);
                _boardsService.RegisterBoardListenerObserver(observer);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
