from utils.socket_utils import recv
import uasyncio
import json
from machine import Pin
from sensors.BreakBeamSensor import BreakBeamSensor
from timers.LocalTimer import LocalTimer
from utils.socket_utils import send


class EventHandler:
    def __init__(
        self,
        reader: uasyncio.StreamReader,
        writer: uasyncio.StreamWriter,
        sensors: list[BreakBeamSensor],
    ):
        self.reader = reader
        self.writer = writer
        self.sensors = sensors
        self.timer = LocalTimer()

    async def run(self):
        for sensor in self.sensors:
            sensor.set_handler_with_ID(self._break_beam_sensor_handler)

        while True:
            command = await recv(self.reader, self.timer, "[command]")
            uasyncio.create_task(self._process_command(command))

    async def _process_command(self, command):
        print("Processing command: " + command)

    # TODO: can it be async?
    async def _break_beam_sensor_handler(self, pin: Pin, pin_id: str):
        response = {
            "pin_id": pin_id,
            "broken": pin.value() == 0,
            "local_timestamp": self.timer.get_current_time(),
        }

        await send(self.writer, self.timer, json.dumps(response))
