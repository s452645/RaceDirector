using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using backend.Services.Hardware;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Hardware
{
    public record AddBoardRequest
    (
        string Id,
        string Address
    );

    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : Controller
    {
        private readonly BoardsManager _boardsManager;

        public BoardController(BoardsManager boardsManager)
        {
            _boardsManager = boardsManager;
        }
/*
        [HttpGet]
        public ActionResult<List<PicoWBoardDto>> GetBoards()
        {
            return Ok(_boardsManager.GetAllBoards());
        }*/

/*        // POST: BoardController/Create
        [HttpPost("addBoard")]
        public async Task<ActionResult> AddBoard(AddBoardRequest request)
        {
            var board = new PicoWBoard(request.Id, request.Address);
            var result = await _boardsManager.AddPicoWBoard(board);

            return result ? Ok() : BadRequest();
        }*/
    }
}
