import uselect
from sys import stdin
from time import sleep

from consts import TICK_DURATION_SECONDS

class SerialController:

  TERMINATOR = "\n"

  def __init__(self):
    self.buffered_input = []
    self.exit = False

  
  def stop(self):
    self.exit = True

  
  def listen_to_serial_input(self, message_handler) -> None:
    if self.exit:
      return

    select_result = uselect.select([stdin], [], [], 0)

    while select_result[0]:
      input_character = stdin.read(1)
      self.buffered_input.append(input_character)
      select_result = uselect.select([stdin], [], [], 0)

    if self.TERMINATOR in self.buffered_input:
      line_ending_index = self.buffered_input.index(self.TERMINATOR)

      message_handler("".join(self.buffered_input[:line_ending_index]).rstrip())

      if line_ending_index < len(self.buffered_input):
        self.buffered_input = self.buffered_input[line_ending_index + 1:]
      else:
        self.buffered_input = []

    sleep(TICK_DURATION_SECONDS)


  def send_to_serial_input(self, message: str):
    print(message)