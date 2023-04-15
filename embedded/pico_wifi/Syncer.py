import json
from utils.socket_utils import recv, send
import uasyncio
from timers.SyncedTimer import SyncedTimer


class Syncer:
    def __init__(
        self,
        reader: uasyncio.StreamReader,
        writer: uasyncio.StreamWriter,
    ):
        self.reader = reader
        self.writer = writer
        self.timer = SyncedTimer()
        self.next_sleep_seconds = 10

    async def run(self):
        while True:
            try:
                print(f"Waiting {self.next_sleep_seconds} s for sync...")
                await uasyncio.sleep(self.next_sleep_seconds)
                print("Sync started...")

                await uasyncio.create_task(self._sync_clock())

            except AssertionError as error:
                print("Error while running syncer:")
                print(error)

    # ===============================================================================

    async def _sync_clock(self):
        _ = await send(
            self.writer,
            self.timer,
            "[1]",
            "ready",
        )

        t1 = 0
        t2 = 0
        t3 = 0
        t4 = 0

        try:
            t1, t2 = await self._sync_packet()
            t3, t4 = await self._delay_packet()

        # TODO: not only, on slower networks (mobile hotspot) it can get out of sync
        # between .NET and board and there is some unrecognized exception thrown
        # add many safety layers for such scenarios, with aborting the connection
        # as the last option which should never happen
        # (.NET & picos should always try to reconnect/resync)
        # also, add more info for frontend for such scenarios
        except AssertionError:
            print("Current sync aborted. Waiting for next.")
            return

        print("t1: " + str(t1))
        print("t2: " + str(t2))
        print("t3: " + str(t3))
        print("t4: " + str(t4))

        t1 = int(t1)
        t2 = int(t2)
        t3 = int(t3)
        t4 = int(t4)

        sync_result = self.timer.calculate_offset(t1, t2, t3, t4)
        response = {
            # commented out until .NET can handle it
            # "t1": t1,
            # "t2": t2,
            # "t3": t3,
            # "t4": t4,
            # "sync_result":
            # {
            "Result": sync_result.result,
            "CurrentSyncOffset": sync_result.offset,
            "LastTenOffsetsAvg": sync_result.avg_offset,
            "NewClockOffset": sync_result.new_clock_offset,
            "ClockAdjustedPicoTimestamp": sync_result.clock_adjusted_pico_timestamp,
            # },
        }

        await send(self.writer, self.timer, "[3]", json.dumps(response))

        if (
            abs(sync_result.offset) < 10
            and sync_result.avg_offset
            and abs(sync_result.avg_offset) < 10
        ):
            self.next_sleep_seconds = 300

        elif abs(sync_result.offset) < 10:
            self.next_sleep_seconds = 120

        elif abs(sync_result.offset) < 20:
            self.next_sleep_seconds = 30

        else:
            self.next_sleep_seconds = 10

    async def _sync_packet(self):
        print("sync_packet start")
        t2, _ = await recv(self.reader, self.timer, "[1]")

        await send(self.writer, self.timer, "[ready-2]", "")

        _, t1 = await recv(self.reader, self.timer, "[2]")

        return t1, t2

    async def _delay_packet(self):
        t3 = await send(self.writer, self.timer, "[2]", "-")
        _, t4 = await recv(self.reader, self.timer, "[3]")

        return t3, t4
