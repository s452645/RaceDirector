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
    ):
        self.sensors: list[BreakBeamSensor] = []
        self.timer = LocalTimer()
        self.messages_to_send_queue = []
        self.background_tasks = list()

    async def run(self, reader: uasyncio.StreamReader, writer: uasyncio.StreamWriter):
        _, payload = await recv(reader, self.timer, "sensors")

        sensors = json.loads(payload)
        for sensor in sensors:
            self.add_sensor(sensor["Pin"], sensor["Id"])

        await send(writer, self.timer, "sensors-done", str(len(sensors)))
        _ = await recv(reader, self.timer, f"board-event-ready")

        task = uasyncio.create_task(self._send_queued_msgs(reader, writer))
        self.background_tasks.append(task)

        # does not work
        # task.add_done_callback(self.background_tasks.remove)

    def add_sensor(self, sensor_pin: int, sensor_id: str | None):
        for sensor in self.sensors:
            if sensor.is_duplicate(sensor_id, sensor_pin):
                return

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
        print("New event: " + str(response))

    async def _send_queued_msgs(
        self, reader: uasyncio.StreamReader, writer: uasyncio.StreamWriter
    ):
        next_event_id = 0

        while True:
            await uasyncio.sleep_ms(5)
            while len(self.messages_to_send_queue) > 0:
                await send(
                    writer,
                    self.timer,
                    f"board-event",
                    self.messages_to_send_queue[0],
                )

                _ = await recv(reader, self.timer, f"board-event-ready")
                self.messages_to_send_queue.pop(0)
                next_event_id += 1
