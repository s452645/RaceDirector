from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, STAY_DOWN_STATE, WAITING_STATE

# logs strategy?

class ArmedState(AbstractState):

  def act(self) -> None:
    self.elevator.motor.stop()


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
    self.elevator.transition_to(WAITING_STATE)

  def finish(self) -> None:
    pass

  def clean_up(self) -> None:
    pass