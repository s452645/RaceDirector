using backend.Exceptions;
using backend.Models;
using backend.Models.Hardware;

namespace backend.Repositories
{
    public class SyncBoardResultRepository : ISyncBoardResultRepository
    {
        private readonly BackendContext _backendContext;

        public SyncBoardResultRepository(BackendContext backendContext) 
        {
            _backendContext = backendContext;
        }

        public SyncBoardResult GetLastSyncBeforeLocalTimestamp(Guid boardId, long localTimestamp)
        {
            var lastSyncBeforeTimestamp = _backendContext.SyncBoardResults
                .Where(sync =>
                    sync.PicoBoardId == boardId &&
                    sync.SyncResult == SyncResult.SYNC_SUCCESS &&
                    sync.ClockAdjustedPicoTimestamp != null &&
                    sync.NewClockOffset != null &&
                    sync.ClockAdjustedPicoTimestamp < localTimestamp)
                .MaxBy(sync => sync.SyncFinishedTimestamp);

            if (lastSyncBeforeTimestamp == null) 
            {
                throw new NotFoundException($"No successfull sync found for board [{boardId}] before [{localTimestamp}] local board timestamp");
            }

            return lastSyncBeforeTimestamp;
        }
    }
}
