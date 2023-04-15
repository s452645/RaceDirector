using System.Text.Json;
using backend.Models;
using backend.Models.Hardware;
using NuGet.Packaging.Signing;

namespace backend.Services.Hardware.Comms
{
    public interface ITimeSyncObserver
    {
        void Notify(SyncBoardResult syncResult);
    }

    public class TimeSyncService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<ITimeSyncObserver> Observers = new List<ITimeSyncObserver>();

        private List<Task> tasks = new List<Task>();


        public TimeSyncService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void Register(ITimeSyncObserver observer)
        {
            Observers.Add(observer);
        }

        public void Unregister(ITimeSyncObserver observer)
        {
            Observers.Remove(observer);
        }

        public void StartListening(PicoWBoard board)
        {
            tasks.Add(Task.Run(async () => await ListenForSyncs(board)));
        }

        private async Task ListenForSyncs(PicoWBoard board)
        {
            var syncSocket = board.SyncSocket;

            while (true && syncSocket.IsConnected())
            {
                try
                {
                    var (response, _) = await syncSocket.Receive("[1]");

                    if (response == "ready")
                    {
                        var result = await SyncBoard(board);
                        var timestamp = DateTime.Now;

                        using var scope = scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

                        result.PicoBoardId = board.PicoBoardDto.Id;

                        if (result.SyncFinishedTimestamp == null)
                        {
                            result.SyncFinishedTimestamp = timestamp;
                        }

                        db.SyncBoardResults.Add(result);
                        db.SaveChanges();

                        Observers.ForEach(o => o.Notify(result));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }



        // TODO: make syncing much more error-proof, inform frontend or at least log every problem
        // but always try to have a fallback and continue syncing process
        /*public void StartSyncing(PicoWBoard picoWBoard)
        {
            var autoEvent = new AutoResetEvent(false);
            var stateTimer = new Timer(
                (stateInfo) => CreateSyncTask(picoWBoard),
                autoEvent, 1000, 30000);
        }*/

        /*public void CreateSyncTask(PicoWBoard picoWBoard)
        {
            var task = Task.Run(async () => await SyncBoard(picoWBoard));

            if (task.Wait(TimeSpan.FromSeconds(10)))
            {
                var timestamp = DateTime.Now;

                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

                var result = task.Result;
                result.PicoBoardId = picoWBoard.PicoBoardDto.Id;

                if (result.SyncFinishedTimestamp == null)
                {
                    result.SyncFinishedTimestamp = timestamp;
                }

                db.SyncBoardResults.Add(result);
                db.SaveChanges();

                Observers.ForEach(o => o.Notify(result));
            }
            // else timeout error in db?
        }*/

        private async Task<SyncBoardResult> SyncBoard(PicoWBoard picoBoard)
        {
            var syncSocket = picoBoard.SyncSocket;

            try
            {
                Thread.Sleep(100);

                var t1 = await syncSocket.Send("[1]", "-");

                _ = await syncSocket.Receive("[ready-2]");

                await syncSocket.Send("[2]", t1.ToString());

                var (_, t4) = await syncSocket.Receive("[2]");
                await syncSocket.Send("[3]", t4.ToString());

                var (results, _) = await syncSocket.Receive("[3]");
                var deserialized = JsonSerializer.Deserialize<SyncBoardResult>(results);

                if (deserialized == null)
                {
                    var timestamp = DateTime.Now;
                    var result = new SyncBoardResult();

                    result.PicoBoardId = picoBoard.PicoBoardDto.Id;
                    result.SyncResult = SyncResult.SYNC_ERROR;
                    result.Message = "Could not deserialize sync results";
                    result.SyncFinishedTimestamp = timestamp;

                    return result;
                }

                return deserialized;
            }
            catch (Exception e)
            {
                var timestamp = DateTime.Now;

                Console.WriteLine("Error while syncing");
                Console.WriteLine(e.ToString());

                var result = new SyncBoardResult();

                result.PicoBoardId = picoBoard.PicoBoardDto.Id;
                result.SyncResult = SyncResult.SYNC_ERROR;
                result.Message = $"Unexpected error: {e.ToString()}";
                result.SyncFinishedTimestamp = timestamp;

                return result;
            }
        }
    }
}
