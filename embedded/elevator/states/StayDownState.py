from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, TEST_GO_UP_STATE, ARMED_STATE

# logs strategy?

class StayDownState(AbstractState):

  def act(self) -> None:
    self.elevator.motor.stop()


  # actions:

  def turn_off(self) -> None:
    self.elevator.transition_to(OFF_STATE)

  def turn_on(self) -> None:
    pass

  def test_up(self) -> None:
    self.elevator.transition_to(TEST_GO_UP_STATE)

  def test_down(self) -> None:
    pass

  def cancel(self) -> None:
    pass

  def arm(self) -> None:
    self.elevator.transition_to(ARMED_STATE)

  def start(self) -> None:
    pass

  def finish(self) -> None:
    pass

  def clean_up(self) -> None:
    pass