from elevator.AbstractState import AbstractState
from elevator.Elevator import OFF_STATE, STAY_DOWN_STATE, STAY_MIDDLE_STATE, STAY_UP_STATE


class PendingState(AbstractState):

  def act(self) -> None:
    self.elevator.motor.stop()
    self._set_position_from_user()


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
    pass

  def arm(self) -> None:
    pass

  def start(self) -> None:
    pass

  def finish(self) -> None:
    pass

  def clean_up(self) -> None:
    pass


  def _set_position_from_user(self) -> None:
    while True:
      print("Describe the current position of the elevator:")
      print("\t0 - DOWN")
      print("\t1 - MIDDLE")
      print("\t2 - UP")
      print()

      position_input = input()

      try:
        position_int = int(position_input)

        if position_int == 0:
          self.elevator.transition_to(STAY_DOWN_STATE)
          return
        elif position_int == 1:
          self.elevator.transition_to(STAY_MIDDLE_STATE)
          return
        elif position_int == 2:
          self.elevator.transition_to(STAY_UP_STATE)
          return
        else:
          print("Invalid position. Please, select one of the values described below:\n")
      except ValueError:
        print("Invalid position. Please, enter a positive integer\n")
