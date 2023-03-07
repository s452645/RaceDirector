using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;

namespace backend.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly TimeSyncService _timeSyncService;

        public WebSocketController(TimeSyncService timeSyncService, PicoWService picoWService)
        {
            _timeSyncService = timeSyncService;

            var board = new PicoWBoard("192.168.1.2", picoWService);
            _timeSyncService.addBoard(board);
        }


        [Route("/sync")]
        public async Task Sync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                _timeSyncService.SyncAll(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
