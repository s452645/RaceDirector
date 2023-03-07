from machine import Pin

class BreakBeamSensor:

  def __init__(self, no_pin: int):
    self._pin = Pin(no_pin, Pin.IN, Pin.PULL_UP)

  def set_handler(self, handler) -> None:
    self._pin.irq(handler=handler, trigger=Pin.IRQ_FALLING | Pin.IRQ_RISING)

  def get_value(self):
    return self._pin.value()