using backend.Exceptions;
using backend.Models;
using backend.Models.Hardware;
using backend.Services.Hardware.Comms;

namespace backend.Services.Hardware
{
    public class BoardsManager
    {
        private readonly IServiceScopeFactory scopeFactory;

        private readonly TimeSyncService _timeSyncService;
        private readonly BoardEventsService _boardEventsService;

        private readonly List<PicoWBoard> _picoWBoards = new();

        public BoardsManager(IServiceScopeFactory scopeFactory, TimeSyncService timeSyncService, BoardEventsService boardEventsService)
        {
            this.scopeFactory = scopeFactory;
            _timeSyncService = timeSyncService;
            _boardEventsService = boardEventsService;
        }

        public async Task<bool> AddPicoWBoard(PicoWBoard picoWBoard)
        {
            var result = await picoWBoard.ConnectSockets();

            if (result)
            {
                _picoWBoards.Add(picoWBoard);
                _timeSyncService.StartListening(picoWBoard);
                await _boardEventsService.AddSensors(picoWBoard);
                _boardEventsService.StartListening(picoWBoard);
            }

            return result;
        }

        public void RunPicoBoardSyncOnce(Guid picoWBoardId)
        {
            var picoWBoard = _picoWBoards.FirstOrDefault(b => b.PicoBoardDto.Id == picoWBoardId);

            if (picoWBoard == null || !picoWBoard.SyncSocket.IsConnected())
            {
                // TODO
                throw new BadRequestException($"Sync failed: Pico W Board [{picoWBoardId}] not found or not connected");
            }

            // _timeSyncService.CreateSyncTask(picoWBoard);
        }

        public void RegisterEventObserver(IBoardEventsObserver observer, bool unregister=false)
        {
            if (unregister)
            {
                _boardEventsService.UnRegister(observer);
                return;
            }

            _boardEventsService.Register(observer);
        }

        public void RegisterTimeSyncObserver(ITimeSyncObserver observer, bool unregister=false)
        {
            if (unregister)
            {
                _timeSyncService.Unregister(observer);
                return;
            }

            _timeSyncService.Register(observer);
        }

        public async Task EmitElevatorEnterEvent(long timestamp)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var sensor = db.BreakBeamSensors.Where(bbs => bbs.Name == "ELEVATOR_DOWN").First();

            var boardEvent = new BoardEvent
            {
                SensorId = sensor.Id,
                Broken = true,
                PicoLocalTimestamp = timestamp,
                ReceivedTimestamp = timestamp
            };

            await _boardEventsService.handleEvent(boardEvent, timestamp);
        }

        public async Task EmitElevatorExitEvent(long timestamp)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var sensor = db.BreakBeamSensors.Where(bbs => bbs.Name == "ELEVATOR_UP").First();

            var boardEvent = new BoardEvent
            {
                SensorId = sensor.Id,
                Broken = true,
                PicoLocalTimestamp = timestamp,
                ReceivedTimestamp = timestamp
            };

            await _boardEventsService.handleEvent(boardEvent, timestamp);

        }
    }
}
