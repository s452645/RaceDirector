from machine import Pin
from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, STAY_DOWN_STATE, CAR_DETECTED_STATE

# logs strategy?

class WaitingState(AbstractState):

  def top_sensor_handler(self, pin: Pin):
    broken = pin.value() == 0
    if broken:
      print("Error: Elevator detected by the top sensor when it was not expected to. Panicking...")
      self.turn_off()


  def bottom_sensor_handler(self, pin: Pin):
    broken = pin.value() == 0
    if broken:
      self.elevator.transition_to(CAR_DETECTED_STATE)


  def act(self) -> None:
    self.elevator.motor.stop()

    self.elevator.set_top_sensor_handler(self.top_sensor_handler)
    self.elevator.set_bottom_sensor_handler(self.bottom_sensor_handler)


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
    self.elevator.transition_to(STAY_DOWN_STATE)

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    self.elevator.transition_to(STAY_DOWN_STATE)

  def clean_up(self) -> None:
    self.elevator.set_top_sensor_handler(lambda _: None)
    self.elevator.set_bottom_sensor_handler(lambda _: None)