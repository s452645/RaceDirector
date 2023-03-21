using System.Text;
using System.Net.WebSockets;
using backend.Models.Hardware;
using System.Text.Json;

namespace backend.Services.Boards.Comms
{
    public enum SyncBoardResult
    {
        SYNC_SUCCESS,
        SYNC_DROPPED,
        SYNC_SUSPICIOUS,
        SYNC_ERROR
    }

    public record SyncBoardResponse
    (
        string BoardId,
        SyncBoardResult Result,
        int? CurrentSyncOffset,
        float? LastTenOffsetsAvg,
        int? NewClockOffset,
        string? Message
    );

    public class TimeSyncService
    {
        private WebSocket? _websocket;
        private TaskCompletionSource<object>? _socketFinishedTcs;
        
        // TODO: make syncing much more error-proof, inform frontend or at least log every problem
        // but always try to have a fallback and continue syncing process
        public void StartSyncingAll(List<PicoWBoard> boards, WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            if (boards.Any(board => !board.IsConnected()))
            {
                Console.WriteLine("Cannot start syncing boards: not all boards are connected.");
                return;
            }

            _websocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;

            var autoEvent = new AutoResetEvent(false);

            var stateTimer = new Timer(
                (stateInfo) => SyncAllBoards(stateInfo, boards),
                autoEvent, 1000, 30000);
        }

        private void SyncAllBoards(object? stateInfo, List<PicoWBoard> boards)
        {
            boards.ForEach(async (board) =>
            {
                // TODO: is it blocking the main thread?
                var task = Task.Run(async () => await SyncBoard(board));

                if (task.Wait(TimeSpan.FromSeconds(10)))
                {
                    SyncBoardResponse response = task.Result;
                    var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    var byteArraySegment = new ArraySegment<byte>(responseBytes, 0, responseBytes.Length);

                    if (_websocket != null)
                    {
                        await _websocket.SendAsync(
                            byteArraySegment,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }
            });
        }

        async private Task<SyncBoardResponse> SyncBoard(PicoWBoard picoWBoard)
        {
            var syncSocket = picoWBoard.SyncSocket;

            if (!syncSocket.IsConnected())
            {
                var msg = $"Sync board failed: Pico W Board [{picoWBoard.Id}] is not connected.";
                return new SyncBoardResponse(picoWBoard.Id, SyncBoardResult.SYNC_ERROR, null, null, null, msg);
            }

            try
            {
                _ = await syncSocket.Send("[-]", "sync");
                var (response, _) = await syncSocket.Receive("[1]");

                if (!response.Equals("ready"))
                {
                    var msg = $"Sync board failed: Pico W Board [{picoWBoard.Id}] sent an unexpected request";
                    return new SyncBoardResponse(picoWBoard.Id, SyncBoardResult.SYNC_ERROR, null, null, null, msg);
                }

                Thread.Sleep(100);

                var t1 = await syncSocket.Send("[1]", "-");
                await syncSocket.Send("[2]", t1.ToString());

                var (_, t4) = await syncSocket.Receive("[2]");
                await syncSocket.Send("[3]", t4.ToString());

                var (results, _) = await syncSocket.Receive("[3]");
                var deserialized = JsonSerializer.Deserialize<SyncBoardResponse>(results);

                if (deserialized == null)
                {
                    return new SyncBoardResponse(picoWBoard.Id, SyncBoardResult.SYNC_ERROR, null, null, null, "Could not deserialize sync results.");
                }

                return deserialized;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while syncing");
                Console.WriteLine(e.ToString());
                return new SyncBoardResponse(picoWBoard.Id, SyncBoardResult.SYNC_ERROR, null, null, null, $"Unexpected error: {e.ToString()}");
            }
        }
    }
}
