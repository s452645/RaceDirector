using backend.Models;
using backend.Models.Hardware;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text.Json;

namespace backend.Services.Hardware.Comms
{
    public interface IBoardEventsObserver
    {
        void Notify(BoardEvent newEvent);
    }

    public record AddSensorCommand
    (
        string command,
        string sensor_id,
        int sensor_pin
    );

    public class BoardEventsService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<Task> tasks = new List<Task>();

        private List<IBoardEventsObserver> Observers = new List<IBoardEventsObserver>();

        public BoardEventsService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void Register(IBoardEventsObserver observer)
        {
            Observers.Add(observer);
        }

        public void UnRegister(IBoardEventsObserver observer)
        {
            Observers.Remove(observer);
        }


        public async Task AddSensors(PicoWBoard board)
        {
            var sensorsCount = board.PicoBoardDto.BreakBeamSensors.Count;

            while (sensorsCount > 0) 
            {
                var sensor = board.PicoBoardDto.BreakBeamSensors[sensorsCount - 1];

                AddSensorCommand command = new AddSensorCommand("ADD_SENSOR", sensor.Id.ToString(), sensor.Pin);
                string serializedCommand = JsonSerializer.Serialize(command);

                _ = await board.EventSocket.Send("[command]", serializedCommand);
                _ = await board.EventSocket.Receive("[command-resp]");

                sensorsCount--;
            }
        }

        public void StartListening(PicoWBoard board)
        {
            tasks.Add(Task.Run(async () => await ListenForEvents(board)));
        }

        private async Task ListenForEvents(PicoWBoard board)
        {
            var eventSocket = board.EventSocket;

            while (true && eventSocket.IsConnected())
            {
                try
                {
                    var (boardEvent, timestampReceived) = await eventSocket.Receive("[event]");

                    var deserialized = JsonSerializer.Deserialize<BoardEvent>(boardEvent);

                    if (deserialized != null)
                    {
                        handleEvent(deserialized, timestampReceived);
                    }

                    await eventSocket.Send("[ready]", "");
                }
                catch(Exception e) 
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void handleEvent(BoardEvent boardEvent, long timestampReceived) 
        {
            boardEvent.ReceivedTimestamp = timestampReceived;

            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

                db.BoardEvents.Add(boardEvent);
                db.SaveChanges();
            }

            Observers.ForEach(o => o.Notify(boardEvent));
        }

        /*public async Task StartListeningForAll(List<PicoWBoard> boards, WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            if (boards.Any(board => !board.IsConnected()))
            {
                Console.WriteLine("Cannot start listening for all events: not all boards are connected.");
                return;
            }

            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;

            var board = boards[0];
            var eventSocket = board.EventSocket;

            while (true)
            {
                var (results, _) = await eventSocket.Receive("[event]");
                Console.WriteLine(results);
            }
*//*
            // listen for events?
            // + process/return them?
            await boards.ForEach(async board =>
            {
               

                *//*if (!eventSocket.IsConnected())
                {
                    var msg = $"Sync board failed: Pico W Board [{board.PicoBoardDto.Id}] is not connected.";
                    return new SyncBoardResponse(picoBoard.PicoBoardDto.Id, SyncBoardResult.SYNC_ERROR, null, null, null, msg);
                }*//*


            });*/
    }
}
