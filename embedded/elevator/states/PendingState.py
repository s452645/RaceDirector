from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, STAY_DOWN_STATE, STAY_MIDDLE_STATE, STAY_UP_STATE


class PendingState(AbstractState):

  def act(self) -> None:
    self.elevator.motor.stop()
    print("Describe the current position of the elevator (DOWN/MIDDLE/UP)")


  # actions:

  def turn_off(self) -> None:
    self.elevator.transition_to(OFF_STATE)

  def turn_on(self) -> None:
    pass

  def test_up(self) -> None:
    self.elevator.transition_to(STAY_UP_STATE)

  def test_down(self) -> None:
    self.elevator.transition_to(STAY_DOWN_STATE)

  def cancel(self) -> None:
    self.elevator.transition_to(STAY_MIDDLE_STATE)

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    pass

  def clean_up(self) -> None:
    pass