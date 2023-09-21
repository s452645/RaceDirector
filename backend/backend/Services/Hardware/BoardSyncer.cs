using backend.Models;
using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using backend.Services.Hardware.Comms;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace backend.Services.Hardware
{
    // TODO: make it IDisposable and close all boards connections? 
    // maybe .NET will call it automatically when closing an app
    class SyncTimes
    {
        public long t1 { get; set; }
        public long t2 { get; set; }
        public long t3 { get; set; }
        public long t4 { get; set; }
    }

    public class PicoTimestamp
    {
        public Guid BoardId { get; set; }
        public long Timestamp { get; set; }

        public PicoTimestamp(Guid boardId, long timestamp)
        {
            BoardId = boardId;
            Timestamp = timestamp;
        }
    }

    public class BoardSyncer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BoardSyncer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<SyncBoardResult> Sync(PicoBoardDto picoBoard)
        {
            var activeBoard = new ActivePicoWBoard(picoBoard);
            await activeBoard.Connect(ConnectionType.SYNC);

            SyncBoardResult result;

            try
            {
                result = await Sync(activeBoard);
            }
            catch (Exception ex)
            {
                result = CatchSyncException(ex, activeBoard);
            }

            activeBoard.Disconnect();
            return result;
        }

        private async Task<SyncBoardResult> Sync(ActivePicoWBoard activeBoard)
        {
            await activeBoard.Send("[0]", string.Empty);
            await activeBoard.Receive("[1]");

            var t1 = (await activeBoard.Send("[1]", "-")).SentTimestamp;
            await activeBoard.Receive("[ready-2]");

            await activeBoard.Send("[2]", t1?.ToString() ?? "null");

            var t4 = (await activeBoard.Receive("[2]")).ReceivedTimestamp;
            await activeBoard.Send("[3]", t4?.ToString() ?? "null");

            var results = (await activeBoard.Receive("[3]")).Content.Message;
            var deserialized = JsonSerializer.Deserialize<SyncTimes>(results);

            if (deserialized == null)
            {
                var timestamp = DateTime.Now;
                var result = new SyncBoardResult();

                result.PicoBoardId = activeBoard.BoardId;
                result.SyncResult = SyncResult.SYNC_ERROR;
                result.Message = "Could not deserialize sync results";
                result.SyncFinishedTimestamp = timestamp;

                return result;
            }

            return await CalculateSyncBoardResult(activeBoard, deserialized);
        }

        private async Task<SyncBoardResult> CalculateSyncBoardResult(ActivePicoWBoard board, SyncTimes syncTimes)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BackendContext>();

            var t2 = await GetSyncedPicoTimestamp(new PicoTimestamp(board.BoardId, syncTimes.t2), db);
            var t3 = await GetSyncedPicoTimestamp(new PicoTimestamp(board.BoardId, syncTimes.t3), db);

            var MS_diff = t2 - syncTimes.t1;
            var SM_diff = syncTimes.t4 - t3;
            var offset = Convert.ToInt64((MS_diff - SM_diff) / 2);

            var previousSyncs = db.SyncBoardResults
                .Where(r => (r.PicoBoardId == board.BoardId) && (SyncResult.SYNC_SUCCESS == r.SyncResult) && (Math.Abs(r.CurrentSyncOffset ?? 0) < 20))
                .OrderByDescending(r => r.SyncFinishedTimestamp)
                .Take(10);

            var previousOffsetsAvgNullable = await previousSyncs
                .AverageAsync(c => c.CurrentSyncOffset);
            var previousOffsetsAvg = previousOffsetsAvgNullable.HasValue ? previousOffsetsAvgNullable.Value : 0;


            var lastClockOffset = await GetCurrentBoardOffset(board.BoardId, db) ?? 0;

            Console.WriteLine($"[{board.BoardId}]: Current offset was [{offset}]");

            var result = new SyncBoardResult();
            result.PicoBoardId = board.BoardId;
            result.CurrentSyncOffset = offset;
            result.LastTenOffsetsAvg = Convert.ToSingle(previousOffsetsAvg);

            if (Math.Abs(offset) > 20)
            {
                if (previousSyncs.ToList().Count() > 5)
                {
                    Console.WriteLine($"[{board.BoardId}]: Assuming it's not representative. Dropping");
                    result.SyncResult = SyncResult.SYNC_DROPPED;
                    result.SyncFinishedTimestamp = DateTime.UtcNow;

                    return result;
                }

                result.SyncResult = SyncResult.SYNC_SUCCESS;
                result.NewClockOffset = lastClockOffset + offset;
                result.SyncFinishedTimestamp = DateTime.UtcNow;
                Console.WriteLine($"[{board.BoardId}]: New clock offset is [{result.NewClockOffset}]");

                return result;
            }

            Console.WriteLine($"[{board.BoardId}]: Average offset is [{previousOffsetsAvg}]");

            result.SyncResult = SyncResult.SYNC_SUCCESS;
            result.NewClockOffset = lastClockOffset + Int32.Parse(Math.Round(previousOffsetsAvg).ToString());
            result.SyncFinishedTimestamp = DateTime.UtcNow;

            Console.WriteLine($"[{board.BoardId}]: New clock offset is [{result.NewClockOffset}]");
            return result;
        }

        private SyncBoardResult CatchSyncException(Exception ex, ActivePicoWBoard activeBoard)
        {
            var timestamp = DateTime.Now;

            Console.WriteLine($"Error while syncing [{activeBoard.BoardId}] Pico W Board:\n{ex}");

            var result = new SyncBoardResult();

            result.PicoBoardId = activeBoard.BoardId;
            result.SyncResult = SyncResult.SYNC_ERROR;
            result.Message = $"Unexpected error: {ex}";
            result.SyncFinishedTimestamp = timestamp;

            return result;
        }

        public static async Task<long?> GetSyncedPicoTimestamp(PicoTimestamp picoTimestamp, BackendContext db)
        {
            var boardOffset = await GetCurrentBoardOffset(picoTimestamp.BoardId, db);
            return picoTimestamp.Timestamp - boardOffset.GetValueOrDefault();
        }

        private static async Task<long?> GetCurrentBoardOffset(Guid boardId, BackendContext db)
        {
            var lastSyncResult = await db.SyncBoardResults
                .Where(r => (r.SyncResult == SyncResult.SYNC_SUCCESS) && (r.PicoBoardId == boardId))
                .OrderByDescending(r => r.SyncFinishedTimestamp)
                .FirstOrDefaultAsync();

            return lastSyncResult?.NewClockOffset;
        }
    }
}
