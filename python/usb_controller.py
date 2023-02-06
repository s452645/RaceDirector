import serial


# modified from https://blog.rareschool.com/2021/01/controlling-raspberry-pi-pico-using.html
class UsbController:
    TERMINATOR = '\n'.encode('UTF8')

    def __init__(self, device='COM3', baud=115200, timeout=1, mock_connection=False):
        self.mock_connection = mock_connection
        self.mock_msg = ''

        if not self.mock_connection:
            self.serial = serial.Serial(device, baud, timeout=timeout)

    def receive(self) -> str:
        if self.mock_connection:
            return self.mock_msg

        line = self.serial.read_until(self.TERMINATOR)
        return line.decode('UTF8').strip()

    def send(self, text: str):
        if self.mock_connection:
            self.mock_msg = f'~~ Mock USB response: {text} ~~'
            return

        line = '%s\n' % text
        self.serial.write(line.encode('UTF8'))

    def close(self):
        if not self.mock_connection:
            self.serial.close()