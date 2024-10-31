# Rui Santos & Sara Santos - Random Nerd Tutorials
# Complete project details at https://RandomNerdTutorials.com/raspberry-pi-pico-stepper-motor-micropython/

import stepper
import time
import gc
from machine import Pin
from controllers.SerialController import SerialController

class Pico:

  def __init__(self):
    self.IN1 = 28
    self.IN2 = 27
    self.IN3 = 26
    self.IN4 = 22
    self.ALT_IN1 = 5
    self.ALT_IN2 = 6
    self.ALT_IN3 = 7
    self.ALT_IN4 = 9
    self.RELAY_ENABLE = 16
    self.ALT_RELAY_ENABLE = 15
      
    self.run_loop = True
    self.serial_controller = SerialController()

    self.pin_relay = Pin(self.RELAY_ENABLE, mode=Pin.OUT)
    self.alt_pin_relay = Pin(self.ALT_RELAY_ENABLE, mode=Pin.OUT)

    self.beam_pin = Pin(17, Pin.IN, Pin.PULL_UP)
    self.alt_beam_pin = Pin(14, Pin.IN, Pin.PULL_UP)
    self.start_beam_pin = Pin(0, Pin.IN, Pin.PULL_UP)
    self.sector_0_beam_pin = Pin(1, Pin.IN, Pin.PULL_UP)
    
    self.stepper_motor = stepper.FullStepMotor.frompins(self.IN1, self.IN2, self.IN3, self.IN4)
    self.alt_stepper_motor = stepper.FullStepMotor.frompins(self.ALT_IN1, self.ALT_IN2, self.ALT_IN3, self.ALT_IN4)
    
    self.beam_pin.irq(handler = self.beam_pin_handler, trigger = Pin.IRQ_FALLING | Pin.IRQ_RISING)
    self.alt_beam_pin.irq(handler = self.alt_beam_pin_handler, trigger = Pin.IRQ_FALLING | Pin.IRQ_RISING)
    self.start_beam_pin.irq(handler = self.start_beam_pin_handler, trigger = Pin.IRQ_FALLING | Pin.IRQ_RISING)
    self.sector_0_beam_pin.irq(handler = self.sector_0_beam_pin_handler, trigger = Pin.IRQ_FALLING | Pin.IRQ_RISING)

  def main(self):
    while self.run_loop:
      self.serial_controller.listen_to_serial_input(self.message_handler)
      time.sleep_ms(1)

  def message_handler(self, message):
    if message == "ACTIVATE_GATE_1":
      self.activate_gate_1()
    elif message == "ACTIVATE_GATE_2":
      self.activate_gate_2()
      
  def activate_gate_1(self):
    self.pin_relay.high()
    time.sleep_ms(50)
    self.stepper_motor.step_until_angle(310)
    time.sleep(1)
    self.stepper_motor.step_until_angle(0)
    time.sleep_ms(50)
    self.pin_relay.low()
    
  def activate_gate_2(self):
    self.alt_pin_relay.high()
    time.sleep_ms(50)
    self.alt_stepper_motor.step_until_angle(310)
    time.sleep(1)
    self.alt_stepper_motor.step_until_angle(0)
    time.sleep_ms(50)
    self.alt_pin_relay.low()
    
  def beam_pin_handler(self, pin):
    if pin.value() == 0:
      print('gate_detected')
      
  def alt_beam_pin_handler(self, pin):
    if pin.value() == 0:
      print('alt_gate_detected')

  def start_beam_pin_handler(self, pin):
    if pin.value() == 0:
      print('start_detected')

  def sector_0_beam_pin_handler(self, pin):
    if pin.value() == 0:
      print('sector_0_detected')


if __name__ == "__main__":
  pico = Pico()
  pico.main()
  gc.collect()


# Set the current position as position 0
# stepper_motor.reset()

# TODO: the stepper library SLEEPS betweens steps.
#		this can impact time reading performance

