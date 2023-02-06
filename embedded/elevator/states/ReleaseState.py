from time import sleep
from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, GO_DOWN_STATE, STAY_UP_STATE
from elevator.consts import RELEASE_DURATION_SECONDS, TICK_DURATION_SECONDS

# logs strategy?

class ReleaseState(AbstractState):

  def __init__(self, release_duration_seconds=RELEASE_DURATION_SECONDS) -> None:
    super().__init__()
    self._release_duration_seconds = release_duration_seconds
    self._abort_countdown = False


  def act(self) -> None:
    self.elevator.motor.stop()

    time_left = self._release_duration_seconds

    while time_left > 0:
      if self._abort_countdown:
        return

      sleep(TICK_DURATION_SECONDS)
      time_left -= TICK_DURATION_SECONDS

    self.elevator.transition_to(GO_DOWN_STATE)


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
    self.elevator.motor.stop()
    self.elevator.transition_to(STAY_UP_STATE)

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    self._abort_countdown = True
    self.elevator.motor.stop()
    self.elevator.transition_to(STAY_UP_STATE)
  
  def clean_up(self) -> None:
    self._abort_countdown = True