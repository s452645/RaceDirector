import json
from sensors.BreakBeamSensor import BreakBeamSensor
from utils.socket_utils import recv
from timers.LocalTimer import LocalTimer
from utils.socket_utils import send
from EventHandler import EventHandler
import uasyncio
import machine
from Syncer import Syncer


class ServerManager:
    def __init__(self):
        self.sensors: list[BreakBeamSensor] = []
        self.timer = LocalTimer()
        self.event_handler = EventHandler()

    async def start_server(self, host: str, port: int):
        server = await uasyncio.start_server(self._handle_connection, host, port)

        print(f"Server started on {host}:{port}")

        return server

    async def _handle_connection(self, reader, writer):
        client_address = writer.get_extra_info("peername")
        print(f"Connection from {client_address}")

        await send(writer, self.timer, "hello", "ready")
        _, connection_type = await recv(reader, LocalTimer(), "conn-type")

        if connection_type == "SYNC":
            await self._handle_sync_connection(reader, writer)
        elif connection_type == "EVENTS":
            await self._handle_event_connection(reader, writer)

        writer.close()

    async def _handle_sync_connection(self, reader, writer):
        await self._accept_conn_type(writer, "SYNC")
        syncer = Syncer(reader, writer)
        await syncer.run()

    async def _handle_event_connection(self, reader, writer):
        await self._accept_conn_type(writer, "EVENTS")
        await self.event_handler.run(reader, writer)

    async def _accept_conn_type(self, writer: uasyncio.StreamWriter, conn_type: str):
        await send(writer, self.timer, "conn-type", conn_type)
