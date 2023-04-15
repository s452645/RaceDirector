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
    ):
        self.reader = reader
        self.writer = writer
        self.sensors: list[BreakBeamSensor] = []
        self.timer = LocalTimer()
        self.messages_to_send_queue = []

    async def run(self):
        uasyncio.create_task(self._send_queued_msgs())

        while True:
            try:
                _, command = await recv(self.reader, self.timer, "[command]")
                uasyncio.create_task(self._process_command(command))

            except Exception as e:
                print("Error processing command")
                print(e)

    async def _process_command(self, command):
        try:
            command = json.loads(command)
        except Exception as e:
            msg = f'Error while parsing command "{command}": {e}'
            print(msg)
            await send(self.writer, self.timer, "[err]", msg)

        if command["command"] == "ADD_SENSOR":
            print("Processing ADD_SENSOR command...")
            self.add_sensor(command["sensor_pin"], command["sensor_id"])

            msg = f"Sensor {command['sensor_id']} added for pin {command['sensor_pin']}"
            print(msg)
            await send(self.writer, self.timer, "[command-resp]", msg)

    def add_sensor(self, sensor_pin: int, sensor_id: str | None):
        sensor = BreakBeamSensor(sensor_pin, sensor_id)
        sensor.set_handler_with_ID(self._break_beam_sensor_handler)
        self.sensors.append(sensor)

    def _break_beam_sensor_handler(self, pin: Pin, sensor_id: str):
        timestamp = self.timer.get_current_time()

        response = {
            "SensorId": sensor_id,
            "Broken": pin.value() == 0,
            "PicoLocalTimestamp": timestamp,
        }

        self.messages_to_send_queue.append(json.dumps(response))

    async def _send_queued_msgs(self):
        while True:
            await uasyncio.sleep_ms(5)
            while len(self.messages_to_send_queue) > 0:
                await send(
                    self.writer,
                    self.timer,
                    "[event]",
                    self.messages_to_send_queue[0],
                )

                try:
                    _ = await recv(self.reader, self.timer, "[ready]")
                    self.messages_to_send_queue.pop(0)

                except AssertionError as error:
                    print("Error while sending event")
                    print(error)
