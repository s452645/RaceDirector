using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class WebSocketController : ControllerBase
    {
        public record AddBoardRequest
        (
            string Id,
            string Address,
            int Port
        );

        private readonly TimeSyncService _timeSyncService;

        public WebSocketController(TimeSyncService timeSyncService)
        {
            _timeSyncService = timeSyncService;

            var board = new PicoWBoard("Board-1", "192.168.1.3", 15000);
            _timeSyncService.addBoard(board);
        }

        [Route("/addBoard")]
        public ActionResult AddBoard([FromBody] AddBoardRequest request)
        {
            var board = new PicoWBoard(request.Id, request.Address, request.Port);
            this._timeSyncService.addBoard(board);
            return Ok();
        }


        [Route("/sync")]
        public async Task Sync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                _timeSyncService.KeepSyncingAll(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
