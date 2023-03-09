from machine import Pin


class BreakBeamSensor:
    def __init__(self, no_pin: int, id: str | None):
        self._pin = Pin(no_pin, Pin.IN, Pin.PULL_UP)
        self._id = str(no_pin) if id is None else id

    def set_handler(self, handler) -> None:
        self._pin.irq(handler=handler, trigger=Pin.IRQ_FALLING | Pin.IRQ_RISING)

    def set_handler_with_ID(self, handler_with_ID):
        handler = lambda pin: handler_with_ID(pin, self._id)
        self.set_handler(handler)

    def get_value(self):
        return self._pin.value()
