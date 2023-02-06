from machine import Pin
from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, STAY_UP_STATE, STAY_MIDDLE_STATE, RELEASE_STATE

# logs strategy?

class GoUpState(AbstractState):

  def __init__(self, test_run: bool):
    self._test_run = test_run


  # will have to change if multi-elevator will have to stop
  # when the beam is broken, wait and then continue until it's not broken
  def top_sensor_handler(self, pin: Pin):
    broken = pin.value() == 0

    if broken:
      self.elevator.motor.stop()

      next_state = STAY_UP_STATE if self._test_run else RELEASE_STATE
      self.elevator.transition_to(next_state)


  def act(self) -> None:
    self.elevator.motor.stop()

    if self.elevator.is_top_sensor_broken():
      print("Error: Elevator detected by the top sensor when it was not expected to. Panicking...")
      self.turn_off()
      return

    self.elevator.set_top_sensor_handler(self.top_sensor_handler)

    self.elevator.motor.go_up()


  # actions:

  def turn_off(self) -> None:
    self.elevator.transition_to(OFF_STATE)

  def turn_on(self) -> None:
    pass

  def test_up(self) -> None:
    pass

  def test_down(self) -> None:
    pass

  def cancel(self) -> None:
    self.elevator.motor.stop()
    self.elevator.transition_to(STAY_MIDDLE_STATE)

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    if not self._test_run:
      self.elevator.motor.stop()
      self.elevator.transition_to(STAY_MIDDLE_STATE)
  
  def clean_up(self) -> None:
    self.elevator.set_top_sensor_handler(lambda _: None)
    self.elevator.set_bottom_sensor_handler(lambda _: None)