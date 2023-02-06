from BreakBeamSensor import BreakBeamSensor
from elevator.AbstractState import AbstractState
from elevator.Motor import Motor
from elevator.states.ArmedState import ArmedState
from elevator.states.GoDownState import GoDownState
from elevator.states.GoUpState import GoUpState
from elevator.states.OffState import OffState
from elevator.states.PendingState import PendingState
from elevator.states.StayDownState import StayDownState
from elevator.states.StayMiddleState import StayMiddleState
from elevator.states.StayUpState import StayUpState
from elevator.states.WaitingState import WaitingState
from elevator.states.ReleaseState import ReleaseState
from elevator.states.CarDetectedState import CarDetectedState 

OFF_STATE = "OFF_STATE"
PENDING_STATE = "PENDING_STATE"
STAY_DOWN_STATE = "STAY_DOWN_STATE"
STAY_MIDDLE_STATE = "STAY_MIDDLE_STATE"
STAY_UP_STATE = "STAY_UP_STATE"
TEST_GO_UP_STATE = "TEST_GO_UP_STATE"
GO_UP_STATE = "GO_UP_STATE"
TEST_GO_DOWN_STATE = "TEST_GO_DOWN_STATE"
GO_DOWN_STATE = "GO_DOWN_STATE"
ARMED_STATE = "ARMED_STATE"
WAITING_STATE = "WAITING_STATE"
CAR_DETECTED_STATE = "CAR_DETECTED_STATE"
RELEASE_STATE = "RELEASE_STATE"

# should elevator count any time at all?
# like time between start and finish, etc.

class Elevator:

  @property
  def motor(self) -> Motor:
    return self._motor


  def __init__(self, motor: Motor, bottom_sensor: BreakBeamSensor, top_sensor: BreakBeamSensor) -> None:
    self._motor = motor

    self._bottom_sensor = bottom_sensor
    self._top_sensor = top_sensor

    self.transition_to(OFF_STATE)


  def transition_to(self, state_str: str):
    state = self._match_state(state_str)

    if self._state is not None:
      self._state.clean_up()

    self._state = state
    self._state.elevator = self

    print("Elevator set to" + state_str)    
    self._state.act()

  
  def set_top_sensor_handler(self, handler):
    self._top_sensor.set_handler(handler)

  
  def set_bottom_sensor_handler(self, handler):
    self._bottom_sensor.set_handler(handler)


  def is_top_sensor_broken(self):
    return self._top_sensor.get_value() == 0

  
  def is_bottom_sensor_broken(self):
    return self._bottom_sensor.get_value() == 0


  # actions:

  def turn_off(self):
    self._state.turn_off()

  def turn_on(self):
    self._state.turn_on()

  def test_up(self):
    self._state.test_up()

  def test_down(self):
    self._state.test_down()

  def cancel(self):
    self._state.cancel()

  def arm(self):
    self._state.arm()

  def start(self):
    self._state.start()

  def finish(self):
    self._state.finish()



  def _match_state(self, state_str: str) -> AbstractState:
    if state_str == OFF_STATE:
      return OffState()
    elif state_str == PENDING_STATE:
      return PendingState()
    elif state_str == STAY_DOWN_STATE:
      return StayDownState()
    elif state_str == STAY_MIDDLE_STATE:
      return StayMiddleState()
    elif state_str == STAY_UP_STATE:
      return StayUpState()
    elif state_str == TEST_GO_DOWN_STATE:
      return GoDownState(test_run=True)
    elif state_str == TEST_GO_UP_STATE:
      return GoUpState(test_run=True)
    elif state_str == ARMED_STATE:
      return ArmedState()
    elif state_str == WAITING_STATE:
      return WaitingState()
    elif state_str == CAR_DETECTED_STATE:
      return CarDetectedState()
    elif state_str == RELEASE_STATE:
      return ReleaseState()
    elif state_str == GO_UP_STATE:
      return GoUpState(test_run=False)
    elif state_str == GO_DOWN_STATE:
      return GoDownState(test_run=False)
      
    else:
      print("Error: request for an unrecognized state. Panicking...")
      return OffState()