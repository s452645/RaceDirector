using backend.Models;
using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using backend.Services.Hardware.Comms;
using System.Text.Json;

namespace backend.Services.Hardware
{
    public interface IBoardListenerObserver
    {
        void Notify(BoardEvent boardEvent);
    }

    public class BoardListener
    {
        private readonly IServiceScopeFactory _scopeFactory;
        
        private Dictionary<string, Task> _listeningTasksById = new();
        private List<IBoardListenerObserver> _observers = new();

        public BoardListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void RegisterObserver(IBoardListenerObserver observer)
        {
            _observers.Add(observer);
        }

        public void UnregisterObserver(IBoardListenerObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task StartListening(PicoBoardDto picoBoard)
        {
            var activeBoard = new ActivePicoWBoard(picoBoard);
            await activeBoard.Connect(ConnectionType.EVENTS);

            var sensorsPayload = JsonSerializer.Serialize(picoBoard.BreakBeamSensors.ToList());
            await activeBoard.Send("sensors", sensorsPayload);
            await activeBoard.Receive("sensors-done");

            _listeningTasksById.Add(activeBoard.BoardId.ToString(), GetListeningTask(activeBoard));
        }

        private async Task GetListeningTask(ActivePicoWBoard activeBoard)
        {
            await activeBoard.Send("board-event-ready", string.Empty);

            var stop = false;
            while (!stop)
            {
                try
                {
                    var boardEvent = await WaitForBoardEvent(activeBoard);
                    await HandleBoardEvent(boardEvent);
                    await activeBoard.Send("board-event-ready", string.Empty);
                }
                catch (Exception ex)
                {
                    stop = true;
                    CatchBoardEventException(ex, activeBoard);
                }
            }
        }

        private async Task<BoardEvent> WaitForBoardEvent(ActivePicoWBoard activeBoard)
        {
            // TODO later: there was an idea to enumerate events in message params
            var message = (await activeBoard.Receive("board-event")).Content.Message;
            var boardEvent = JsonSerializer.Deserialize<BoardEvent>(message);

            if (boardEvent == null)
            {
                throw new Exception($"Error while handling event of [{activeBoard.BoardId}] Pico W Board: cannot deserialize Event");
            }

            return boardEvent;
        }

        private async Task HandleBoardEvent(BoardEvent boardEvent)
        {
            _observers.ForEach(observer => observer.Notify(boardEvent));

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            db.BoardEvents.Add(boardEvent);
            await db.SaveChangesAsync();
        }

        private void CatchBoardEventException(Exception ex, ActivePicoWBoard activeBoard)
        {
            Console.WriteLine($"Error while handling events for [{activeBoard.BoardId}] Pico W Board:\n{ex}");
        }
    }
}
