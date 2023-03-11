using backend.Models.Hardware;
using backend.Services.Boards;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class WebSocketController : ControllerBase
    {
        public record AddBoardRequest
        (
            string Id,
            string Address
        );

        private readonly BoardsManager _boardsManager;

        public WebSocketController(BoardsManager boardsManager)
        {
            this._boardsManager = boardsManager;
        }

        [Route("/addBoard")]
        public async Task<ActionResult> AddBoard([FromBody] AddBoardRequest request)
        {
            var board = new PicoWBoard(request.Id, request.Address);
            var result = await _boardsManager.AddPicoWBoard(board);
            
            return Ok();
        }


        [Route("/sync")]
        public async Task Sync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                _boardsManager.LaunchSyncAllBoards(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/events")]
        public async Task HandleEvents()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();

            _boardsManager.LaunchEventHandlingAllBoards(webSocket, socketFinishedTcs);
        }
    }
}
