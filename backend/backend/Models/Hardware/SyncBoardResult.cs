using System.Numerics;

namespace backend.Models.Hardware
{
    public enum SyncResult
    {
        SYNC_SUCCESS,
        SYNC_DROPPED,
        SYNC_SUSPICIOUS,
        SYNC_ERROR
    }

    public class SyncBoardResult
    {
        public Guid Id { get; set; }

        public Guid PicoBoardId { get; set; }
        public PicoBoard PicoBoard { get; set; }

        public SyncResult SyncResult { get; set; }
        public Int64? CurrentSyncOffset { get; set; }
        public float? LastTenOffsetsAvg { get; set; }
        public Int64? NewClockOffset { get; set; }
        public string? Message { get; set; }

        public Int64? ClockAdjustedPicoTimestamp { get; set; }
        public DateTime? SyncFinishedTimestamp { get; set; }
    }
}
