class AbstractState:

  @property
  def elevator(self):
    return self._elevator

  @elevator.setter
  def elevator(self, elevator) -> None:
    self._elevator = elevator

  def act(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")


  # actions:

  def turn_off(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def turn_on(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def test_up(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def test_down(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def cancel(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def arm(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def start(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def finish(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")

  def clean_up(self) -> None:
    raise NotImplementedError("This is an abstract method, it must be implemented in a subclass")