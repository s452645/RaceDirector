using backend.Models.Hardware;

namespace backend.Repositories
{
    public interface ISyncBoardResultRepository
    {
        public SyncBoardResult GetLastSyncBeforeLocalTimestamp(Guid boardId, long timestamp);
    }
}
