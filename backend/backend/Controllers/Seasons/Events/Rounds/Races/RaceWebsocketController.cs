using backend.Models.Dtos.Seasons.Events.Rounds.Races;
using backend.Services.Hardware.Comms;
using backend.Services.Seasons.Events.Rounds.Races;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Seasons.Events.Rounds.Races
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RaceWebsocketController : ControllerBase
    {
        private readonly RaceManager _raceManager;

        public RaceWebsocketController(RaceManager raceManager)
        {
            _raceManager = raceManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("race")]
        public async Task RaceWebsocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                var socket = new RaceDirectorWebSocket<SeasonEventRoundRaceDto>(webSocket, socketFinishedTcs);
                _raceManager.RegisterWebsocket(socket);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
