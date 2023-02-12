import socket
import time


class SocketController:

    def __init__(self, port=6655, max_connections=2):
        self.host = socket.gethostname()
        self.port = port
        self.socket = socket.socket()

        self.socket.bind((self.host, self.port))
        self.socket.listen(max_connections)

        self.connection, self.address = self.socket.accept()
        print("Connection from: " + str(self.address))

        self.connected = True

    def listen(self, data_handler):
        while self.connected:
            data = self.connection.recv(4096).decode()
            data_handler(data)
            # investigate
            time.sleep(0.1)

    def send(self, data):
        self.connection.send(data.encode())

    def exit(self):
        self.connected = False
        self.connection.close()
