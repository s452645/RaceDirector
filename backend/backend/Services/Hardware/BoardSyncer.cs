using backend.Models.Dtos.Hardware;
using backend.Models.Hardware;
using backend.Services.Hardware.Comms;
using System.Text.Json;

namespace backend.Services.Hardware
{
    // TODO: make it IDisposable and close all boards connections? 
    // maybe .NET will call it automatically when closing an app
    public class BoardSyncer
    {
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
            var deserialized = JsonSerializer.Deserialize<SyncBoardResult>(results);

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

            deserialized.PicoBoardId = activeBoard.BoardId;
            return deserialized;
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
    }
}
