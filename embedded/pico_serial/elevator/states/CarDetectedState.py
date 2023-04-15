from time import sleep
from machine import Pin
from consts import TICK_DURATION_SECONDS
from elevator.AbstractState import AbstractState
from elevator.consts import (
    GO_UP_STATE,
    LIFT_DELAY_SECONDS,
    OFF_STATE,
    STAY_DOWN_STATE,
    WAITING_STATE,
)

# logs strategy?


class CarDetectedState(AbstractState):
    def __init__(self, lift_delay_seconds=LIFT_DELAY_SECONDS):
        super().__init__()
        self._lift_delay_seconds = lift_delay_seconds
        self._abort_countdown = False

    def bottom_sensor_handler(self, pin: Pin):
        broken = pin.value() == 0
        if not broken:
            print("Oops, the car is no longer detected. Aborting the launch.")
            self.elevator.transition_to(WAITING_STATE)

    def act(self) -> None:
        self.elevator.motor.stop()

        # no need for that, some cars are just to short
        # self.elevator.set_bottom_sensor_handler(self.bottom_sensor_handler)

        while True:
            time_left_seconds = self._lift_delay_seconds

            while time_left_seconds > 0:
                if self._abort_countdown:
                    return

                sleep(TICK_DURATION_SECONDS)
                time_left_seconds -= TICK_DURATION_SECONDS

            self.elevator.transition_to(GO_UP_STATE)

    # actions:

    def turn_off(self) -> None:
        self._abort_countdown = True
        self.elevator.transition_to(OFF_STATE)

    def turn_on(self) -> None:
        pass

    def test_up(self) -> None:
        pass

    def test_down(self) -> None:
        pass

    def cancel(self) -> None:
        self._abort_countdown = True
        self.elevator.transition_to(STAY_DOWN_STATE)

    def arm(self) -> None:
        pass

    def start(self) -> None:
        pass

    def finish(self) -> None:
        self._abort_countdown = True
        self.elevator.transition_to(STAY_DOWN_STATE)

    def clean_up(self) -> None:
        self._abort_countdown = True
        self.elevator.set_bottom_sensor_handler(lambda _: None)
