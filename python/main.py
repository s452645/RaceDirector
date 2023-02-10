from SocketController import SocketController
from UsbController import UsbController
from serial.serialutil import SerialException


class MainProgram:
    def __init__(self):
        self.isRunning = False

        self.usb_controller = UsbController(mock_connection=True)
        self.socket_controller = SocketController()

    def socket_received_data_handler(self, data: str):
        # a good place to perform some ifs
        self.usb_controller.send(data)

        # ideally it should listen for usb messeges constanty,
        # not only as a response
        # usb_response = self.usb_controller.receive()

        # a good place to perform some ifs
        # self.socket_controller.send(usb_response)

        if data == "EXIT":
            self.exit()
            return

    def serial_received_data_handler(self, data: str):
        # self.socket_controller.send(data)
        # for the time being
        print(data)

    def run(self):
        self.isRunning = True
        self.socket_controller.listen(self.socket_received_data_handler)
        self.usb_controller.listen_to_serial(self.serial_received_data_handler)

    def exit(self):
        if not self.isRunning:
            print("Program has already exited")
            return

        self.isRunning = False
        self.socket_controller.exit()
        self.usb_controller.close()


if __name__ == "__main__":
    program = None

    try:
        program = MainProgram()
        program.run()
    except SerialException as e:
        print("Error: SerialException")
        print(str(e))
    finally:
        if program is not None:
            program.exit()
