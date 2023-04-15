import json
import time

from threading import Thread

from SocketController import SocketController
from UsbController import UsbController
from serial.serialutil import SerialException


class MainProgram:
    def __init__(self):
        self.isRunning = False

        self.usb_controller = UsbController(mock_connection=False)
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
        timestamp = round(time.time() * 1000)

        msg = None
        if "car_detected_state" in data.strip().lower():
            msg = "car_detected"
        elif "release_state" in data.strip().lower():
            msg = "release"

        if msg is not None:
            print("Sending message " + msg)
            self.socket_controller.send(json.dumps({'msg': msg, 'timestamp': timestamp}))

        print(data)

    def run(self):
        self.isRunning = True

        t1 = Thread(target=self.socket_controller.listen, args=[self.socket_received_data_handler])
        t2 = Thread(target=self.usb_controller.listen_to_serial, args=[self.serial_received_data_handler])

        t1.start()
        t2.start()

        t1.join()
        t2.join()

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
