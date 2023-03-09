import json
from utils.socket_utils import recv, send
import uasyncio
from timers.SyncedTimer import SyncedTimer


class Syncer:
    def __init__(
        self,
        reader: uasyncio.StreamReader,
        writer: uasyncio.StreamWriter,
        timer: SyncedTimer,
    ):
        self.reader = reader
        self.writer = writer
        self.timer = timer

    async def run(self):
        while True:
            _, request = await recv(self.reader, self.timer, "[-]")

            if request == "sync":
                print("Sync started...")
                await uasyncio.wait_for_ms(self._sync_clock(), 1500)
            else:
                print("Unknown command: ", str(request))

    # ===============================================================================

    async def _sync_clock(self):
        await send(self.writer, self.timer, "ready")

        t1 = 0
        t2 = 0
        t3 = 0
        t4 = 0

        try:
            t1, t2 = await self._sync_packet()
            t3, t4 = await self._delay_packet()
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
            "t1": t1,
            "t2": t2,
            "t3": t3,
            "t4": t4,
            "sync_result": {
                "result": sync_result.result,
                "offset": sync_result.offset,
                "avg_offset": sync_result.avg_offset,
                "new_clock_offset": sync_result.new_clock_offset,
            },
        }

        await send(self.writer, self.timer, json.dumps(response))

    async def _sync_packet(self):
        t2, _ = await recv(self.reader, self.timer, "[1]")
        _, t1 = await recv(self.reader, self.timer, "[2]")

        return t1, t2

    async def _delay_packet(self):
        t3 = await send(self.writer, self.timer, "-")
        _, t4 = await recv(self.reader, self.timer, "[3]")

        return t3, t4
