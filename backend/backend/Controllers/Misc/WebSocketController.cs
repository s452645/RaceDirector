using backend.Services.Hardware;
using backend.Services.Hardware.Comms;
using backend.Services.Seasons.Events.Rounds.Races;
using backend.Services.Seasons.Events.Rounds.Races.Heats;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Misc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebSocketController : ControllerBase
    {
        private readonly BoardsManager _boardsManager;
        private readonly SeasonEventRoundRaceService _raceService;

        public WebSocketController(BoardsManager boardsManager, SeasonEventRoundRaceService raceService)
        {
            _boardsManager = boardsManager;
            _raceService = raceService;
        }

        [Route("/sync")]
        public async Task Sync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                var observer = new TimeSyncObserver(webSocket, socketFinishedTcs);
                _boardsManager.RegisterTimeSyncObserver(observer);

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

            var observer = new BoardEventWebsocketObserver(webSocket, socketFinishedTcs);
            _boardsManager.RegisterEventObserver(observer);

            await socketFinishedTcs.Task;
        }

        [Route("/heat")]
        public async Task ListenToHeat()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();


            var observer = new HeatWebSocketObserver(webSocket, socketFinishedTcs);
            _raceService.RegisterHeatObserver(observer);

            await socketFinishedTcs.Task;
        }
    }
}
