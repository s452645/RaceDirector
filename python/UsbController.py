import serial
import time


class UsbController:
    TERMINATOR = '\n'.encode('UTF8')

    def __init__(self, device='COM4', baud=115200, timeout=1, mock_connection=False):
        self.mock_connection = mock_connection
        self.mock_msg = ''
        self.exit = False

        if not self.mock_connection:
            self.serial = serial.Serial(device, baud, timeout=timeout)

    def listen_to_serial(self, message_handler):
        if self.mock_connection:
            return

        while True:
            if self.exit:
                return

            if self.serial.in_waiting > 0:
                line = self.serial.read_until(self.TERMINATOR)
                if line.strip():
                    message_handler(line.decode('UTF8').strip())

            time.sleep(0.1)

    def receive(self) -> str:
        if self.mock_connection:
            return self.mock_msg

        line = self.serial.read_until(self.TERMINATOR)
        return line.decode('UTF8').strip()

    def send(self, text: str):
        if self.mock_connection:
            print("Mock message sent to Pico: " + text)
            self.mock_msg = f'~~ Mock USB response: {text} ~~'
            return

        line = '%s\n' % text
        self.serial.write(line.encode('UTF8'))
        print("Message sent to Pico: " + line)

    def close(self):
        self.exit = True
        if not self.mock_connection:
            self.serial.close()