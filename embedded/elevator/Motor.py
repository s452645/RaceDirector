from machine import PWM, Pin
import time

from consts import TICK_DURATION_SECONDS

MOTOR_MODE_STOP = "MOTOR_MODE_STOP"
MOTOR_MODE_UP = "MOTOR_MODE_UP"
MOTOR_MODE_DOWN = "MOTOR_MODE_DOWN"

class Motor:
  
  def __init__(
    self, 
    no_pin1: int, 
    no_pin2: int,
    no_pin_speed: int, 
    pin_speed_freq = 1000, 
    tick_duration_s = TICK_DURATION_SECONDS
  ):
    self._pinIN1 = Pin(no_pin1, Pin.OUT)
    self._pinIN2 = Pin(no_pin2, Pin.OUT)
    self._pin_speed = PWM(Pin(no_pin_speed))
    self._pin_speed.freq(pin_speed_freq)

    self._motor_mode = MOTOR_MODE_STOP
    self._tick_duration_s = tick_duration_s


  def stop(self):
    if self._motor_mode == MOTOR_MODE_STOP:
      print("Relax, the motor is already stopped. (I guess?)")
      return

    print("Motor is stopping")

    self._pinIN1.low()
    self._pinIN2.low()
    self._motor_mode = MOTOR_MODE_STOP


  def go_up(self, speed=40000, duration_s=None):
    print("Motor is going up (speed: " + str(speed) + " duration: " + str("-" if duration_s is None else duration_s))

    self._motor_mode = MOTOR_MODE_UP
    self._pin_speed.duty_u16(speed)
    self._pinIN1.high()
    self._pinIN2.low()

    if duration_s is not None:
      self._stop_after_s(duration_s)


  def go_down(self, speed=40000, duration_s=None):
    print("Motor is going down (speed: " + str(speed) + " duration: " + str("-" if duration_s is None else duration_s))

    self._motor_mode = MOTOR_MODE_DOWN
    self._pin_speed.duty_u16(speed)
    self._pinIN1.low()
    self._pinIN2.high()

    if duration_s is not None:
      self._stop_after_s(duration_s)


  def _stop_after_s(self, duration_s):
    time_left_s = duration_s
    while time_left_s > 0 and self._motor_mode is not MOTOR_MODE_STOP:
      time.sleep(self._tick_duration_s)
      time_left_s -= self._tick_duration_s

    self.stop()