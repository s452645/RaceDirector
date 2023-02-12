from elevator.AbstractState import AbstractState
from elevator.consts import PENDING_STATE

# logs strategy?

class OffState(AbstractState):

  def act(self) -> None:
    self.elevator.motor.stop()


  # actions:

  def turn_off(self) -> None:
    pass

  def turn_on(self) -> None:
    self.elevator.transition_to(PENDING_STATE)

  def test_up(self) -> None:
    pass

  def test_down(self) -> None:
    pass

  def cancel(self) -> None:
    pass

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    pass

  def clean_up(self) -> None:
    pass