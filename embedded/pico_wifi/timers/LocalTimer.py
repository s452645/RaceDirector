import time
from timers.AbstractTimer import AbstractTimer


class LocalTimer(AbstractTimer):
    def get_current_time(self):
        return time.ticks_ms()

