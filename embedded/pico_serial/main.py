import time
import gc
from controllers.SerialController import SerialController
from elevator.ElevatorController import ElevatorController

from machine import Pin

from consts import EXIT_COMMAND, TICK_DURATION_SECONDS
from elevator.consts import BOTTOM_SENSOR_PIN, MOTOR_PIN_1, MOTOR_PIN_2, MOTOR_PIN_SPEED, UPPER_SENSOR_PIN
\
class Pico:

  def __init__(self):
    self.run_loop = True
    self.elevator_controller = ElevatorController(
      MOTOR_PIN_1, 
      MOTOR_PIN_2, 
      MOTOR_PIN_SPEED, 
      UPPER_SENSOR_PIN,
      BOTTOM_SENSOR_PIN
    )

    self.serial_controller = SerialController()
    self.led = Pin(25, Pin.OUT)
    self.led.value(0)
    
    
  def main(self):

    while self.run_loop:
      self.serial_controller.listen_to_serial_input(self.message_handler)
      time.sleep(TICK_DURATION_SECONDS)


  def exit(self):
    self.serial_controller.stop()
    self.run_loop = False

  
  def message_handler(self, message):
    if self.led.value():
      self.led.value(0)
    else:
      self.led.value(1)

    if message == EXIT_COMMAND:
      self.exit()
      return

    self.elevator_controller.process_command(message)


if __name__ == "__main__":
  pico = Pico()
  pico.main()
  gc.collect()