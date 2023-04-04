using backend.Models.Hardware;
using backend.Services.Hardware;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Misc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebSocketController : ControllerBase
    {
        private readonly BoardsManager _boardsManager;

        public WebSocketController(BoardsManager boardsManager)
        {
            _boardsManager = boardsManager;
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
