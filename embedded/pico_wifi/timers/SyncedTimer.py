import time
from timers.AbstractTimer import AbstractTimer

SYNC_SUCCESS = "SYNC_SUCCESS"
SYNC_DROPPED = "SYNC_DROPPED"
SYNC_SUSPICIOUS = "SYNC_SUSPICIOUS"


class TimerSyncResponse:
    def __init__(
        self,
        result: str,
        offset: int,
        avg_offset: float,
        new_clock_offset: int,
        clock_adjusted_pico_timestamp: int | None,
    ):
        self.result = result
        self.offset = offset
        self.avg_offset = avg_offset
        self.new_clock_offset = new_clock_offset
        self.clock_adjusted_pico_timestamp = clock_adjusted_pico_timestamp


class SyncedTimer(AbstractTimer):
    def __init__(self):
        self.previous_offsets = []
        self.clock_offset = 0
        self.last_sync = self.get_current_time()

    def calculate_offset(self, t1, t2, t3, t4) -> TimerSyncResponse:
        MS_diff = t2 - t1
        SM_diff = t4 - t3

        offset = int((MS_diff - SM_diff) / 2)
        previous_offsets_count = len(self.previous_offsets)

        avg_offset = 0
        if previous_offsets_count > 0:
            avg_offset = sum(self.previous_offsets) / previous_offsets_count

        print("Current offset was ", str(offset))

        if abs(offset) > 20:
            if previous_offsets_count > 5:
                print("Assuming it's not representative. No time adjustments.")
                return TimerSyncResponse(
                    SYNC_DROPPED, offset, avg_offset, self.clock_offset, None
                )

            self.clock_offset = self.clock_offset + offset

            print("New offset is ", str(self.clock_offset), "\n")
            return TimerSyncResponse(
                SYNC_SUCCESS, offset, avg_offset, self.clock_offset, None
            )

        # TODO: enter sus mode (request more syncs)
        # if next are also sus, display a warning on frontend
        if abs(offset) > 5:
            if previous_offsets_count == 10:
                is_avg_trustworthy = abs(avg_offset) < 3

                if is_avg_trustworthy:
                    print(
                        "Suspicious. Avg probably could be trusted more. No time adjustments."
                    )
                    return TimerSyncResponse(
                        SYNC_SUSPICIOUS, offset, avg_offset, self.clock_offset, None
                    )

        self.previous_offsets.append((offset))

        offsets_count = len(self.previous_offsets)
        if offsets_count > 10:
            self.previous_offsets.pop(0)
            offsets_count -= 1

        print("Previous offsets: ", self.previous_offsets)
        print("Average offset: ", avg_offset)

        self.clock_offset = self.clock_offset + int(avg_offset)
        clock_adjusted_timestamp = self.get_current_local_time()

        print("New offset is ", str(self.clock_offset), "\n")

        return TimerSyncResponse(SYNC_SUCCESS, offset, avg_offset, self.clock_offset, clock_adjusted_timestamp)

    def get_current_time(self):
        return time.ticks_ms() - self.clock_offset

    def get_current_local_time(self):
        return time.ticks_ms()
