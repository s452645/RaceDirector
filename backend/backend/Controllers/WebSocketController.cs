using backend.Models.Hardware;
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

        private readonly HardwareWifiCommunicationService _wifiCommsService;

        public WebSocketController(HardwareWifiCommunicationService wifiCommsService)
        {
            _wifiCommsService = wifiCommsService;
        }

        [Route("/addBoard")]
        public async Task<ActionResult> AddBoard([FromBody] AddBoardRequest request)
        {
            var board = new PicoWBoard(request.Id, request.Address, request.Port);
            var result = await _wifiCommsService.AddAndConnectBoard(board);
            
            // how to return sth else?
            return result ? Ok() : BadRequest();
        }


        [Route("/sync")]
        public async Task Sync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                _wifiCommsService.SyncAllForever(webSocket, socketFinishedTcs);

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

            _wifiCommsService
        }
    }
}
