using Microsoft.AspNetCore.Mvc;
using backend.Models.Dtos;
using backend.Services;

namespace backend.Controllers
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
    }
}
