from sensors.BreakBeamSensor import BreakBeamSensor
from EventHandler import EventHandler
from timers.SyncedTimer import SyncedTimer
import uasyncio
from Syncer import Syncer
import time
import machine

class ServerManager:
   
    def __init__(self):
        self.pending = 2
        self.led_task = uasyncio.create_task(self._blink_led())
        self.led = machine.Pin("LED", machine.Pin.OUT)
    
    
    async def launch_sync_server(self, host: str, port: int):
        sync_server = await uasyncio.start_server(
            self._handle_sync_connection, host, port
        )

        print(f"[SYNC] Server started on {host}:{port}")
        
        async with sync_server:
            while True:
                print("sleep")
                await uasyncio.sleep(5)        
        
    async def launch_event_server(
        self,
        sensors: list[BreakBeamSensor],
        host: str,
        port: int,
    ):
        self.sensors = sensors

        event_server = await uasyncio.start_server(
            self._handle_event_connection, host, port
        )

        print(f"[EVENT] Server started on {host}:{port}")

        async with event_server:
            while True:
                print("sleep event")
                await uasyncio.sleep(5)

    async def _handle_sync_connection(self, reader, writer):
        client_address = writer.get_extra_info('peername')
        print(f"[SYNC] Connection from {client_address}")
        self.pending -= 1

        try:
            syncer = Syncer(reader, writer, SyncedTimer())
            await syncer.run()

        # TODO?
        except Exception as e:
            print("Error while running syncer")
            print(e)
            # self._handle_server_error(command_socket, e)
            # return

    async def _handle_event_connection(self, reader, writer):
        client_address = writer.get_extra_info('peername')
        print(f"[EVENT] Connection from {client_address}")
        self.pending -= 1

        try:
            event_handler = EventHandler(reader, writer, self.sensors)
            await event_handler.run()

        # TODO?
        except Exception as e:
            print("Error while running event handler")
            print(e)
            # self._handle_socket_error(event_socket, e)
            # return
            
    async def _blink_led(self):
        while self.pending > 0:
            self.led.on()
            await uasyncio.sleep(self.pending)
            self.led.off()
            await uasyncio.sleep(self.pending)



