import json
import time

from threading import Thread

from SocketController import SocketController
from UsbController import UsbController
from serial.serialutil import SerialException


class MainProgram:
    def __init__(self):
        self.isRunning = False
        self.lastMessage = ""

        self.usb_controller1 = UsbController(device='COM6', mock_connection=False)
        self.usb_controller2 = UsbController(device='COM8', mock_connection=False)
        self.socket_controller = SocketController()

    def socket_received_data_handler(self, data: str):
        # a good place to perform some ifs

        if data.startswith("ELEVATOR->"):
            message = data.replace("ELEVATOR->", "")
            self.usb_controller1.send(message)
        elif data.startswith("BARRIERS->"):
            message = data.replace("BARRIERS->", "")
            self.usb_controller2.send(message)

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
        if "gate_detected" in data.strip().lower() and "alt" not in data.strip().lower():
            msg = "gate_detected"
        elif "alt_gate_detected" in data.strip().lower():
            msg = "alt_gate_detected"
        elif "start_detected" in data.strip().lower():
            msg = "start_detected"
        elif "sector_0_detected" in data.strip().lower():
            msg = "sector_0_detected"
        elif "old-resume" in data.strip().lower():
            msg = "old-resume"
        elif "alt-resume" in data.strip().lower():
            msg = "alt-resume"

        # photofinish case
        elif "@" in data.strip().lower():
            msg = data

        if msg is not None and msg != self.lastMessage:
            print("Sending message " + msg)
            self.socket_controller.send(json.dumps({'msg': msg, 'timestamp': timestamp}))
            self.lastMessage = msg

        # print(data)

    def run(self):
        self.isRunning = True

        t1 = Thread(target=self.socket_controller.listen, args=[self.socket_received_data_handler])
        t2 = Thread(target=self.usb_controller1.listen_to_serial, args=[self.serial_received_data_handler])
        t3 = Thread(target=self.usb_controller2.listen_to_serial, args=[self.serial_received_data_handler])

        t1.start()
        t2.start()
        t3.start()

        t1.join()
        t2.join()
        t3.join()

    def exit(self):
        if not self.isRunning:
            print("Program has already exited")
            return

        self.isRunning = False
        self.socket_controller.exit()
        self.usb_controller1.close()
        self.usb_controller2.close()


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
