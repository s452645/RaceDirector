from timers.LocalTimer import LocalTimer
from utils.socket_utils import send
from EventHandler import EventHandler
import uasyncio
from Syncer import Syncer
import machine


class ServerManager:
    def __init__(self):
        self.pending = 0
        self.led_task = uasyncio.create_task(self._blink_led())
        self.led = machine.Pin("LED", machine.Pin.OUT)

    async def launch_sync_server(self, host: str, port: int):
        sync_server = await uasyncio.start_server(
            self._handle_sync_connection, host, port
        )

        print(f"[SYNC] Server started on {host}:{port}")
        self.pending += 1

        async with sync_server:
            while True:
                await uasyncio.sleep(60)

    async def launch_event_server(
        self,
        host: str,
        port: int,
    ):
        event_server = await uasyncio.start_server(
            self._handle_event_connection, host, port
        )

        print(f"[EVENT] Server started on {host}:{port}")
        self.pending += 1

        async with event_server:
            while True:
                await uasyncio.sleep(60)

    async def _handle_sync_connection(self, reader, writer):
        client_address = writer.get_extra_info("peername")
        print(f"[SYNC] Connection from {client_address}")
        self.pending -= 1

        while True:
            try:
                syncer = Syncer(reader, writer)
                await syncer.run()

            except Exception as e:
                print("Unknown Error while running syncer:")
                print(e.with_traceback)

                # for what?
                # await self._handle_server_error(writer, e)

    async def _handle_event_connection(self, reader, writer):
        client_address = writer.get_extra_info("peername")
        print(f"[EVENT] Connection from {client_address}")
        self.pending -= 1

        try:
            event_handler = EventHandler(reader, writer)
            await event_handler.run()

        except Exception as e:
            print("Error while running event handler")
            print(e)
            await self._handle_server_error(writer, e)

    async def _handle_server_error(self, writer, err):
        # await send(writer, LocalTimer(), "[err]", str(err))
        print("Closing connection")
        writer.close()
        print("Connection closed")

    async def _blink_led(self):
        while self.pending != 2:
            await uasyncio.sleep(1)

        while self.pending > 0:
            self.led.on()
            await uasyncio.sleep(self.pending)
            self.led.off()
            await uasyncio.sleep(self.pending)
