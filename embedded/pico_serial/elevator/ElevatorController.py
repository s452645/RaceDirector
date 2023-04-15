from sensors.BreakBeamSensor import BreakBeamSensor
from elevator.Elevator import Elevator
from elevator.Motor import Motor

COMMAND_ON = "ON"
COMMAND_OFF = "OFF"
COMMAND_GO_UP = "GO_UP"
COMMAND_GO_DOWN = "GO_DOWN"
COMMAND_ARM = "ARM"
COMMAND_START = "START"
COMMAND_FINISH = "FINISH"
COMMAND_CANCEL = "CANCEL"


class ElevatorController:

  def __init__(
    self, 
    motor_pin2: int,
    motor_pin1: int, 
    motor_pin_speed: int, 
    upper_sensor_pin: int,
    lower_sensor_pin: int):

    motor = Motor(motor_pin1, motor_pin2, motor_pin_speed)
    upper_sensor = BreakBeamSensor(upper_sensor_pin)
    lower_sensor = BreakBeamSensor(lower_sensor_pin)

    self.elevator = Elevator(motor, lower_sensor, upper_sensor)


  def process_command(self, command: str):
    if command == COMMAND_OFF:
      self.elevator.turn_off()
    elif command == COMMAND_ON:
      self.elevator.turn_on()
    elif command == COMMAND_GO_UP:
      self.elevator.test_up()
    elif command == COMMAND_GO_DOWN:
      self.elevator.test_down()
    elif command == COMMAND_ARM:
      self.elevator.arm()
    elif command == COMMAND_START:
      self.elevator.start()
    elif command == COMMAND_FINISH:
      self.elevator.finish()
    elif command == COMMAND_CANCEL:
      self.elevator.cancel()
    else:
      print("Unknown command: " + str(command))