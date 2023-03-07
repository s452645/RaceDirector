import socket
import uasyncio
import time
from Syncer import Syncer


class SocketManager:
    tasks = []

    async def launch_sync_socket(self, address: str, port: int = 80):
        sync_socket = self._init_TCP_client_socket(port, address)

        if sync_socket is None:
            return False

        self.tasks.append(uasyncio.create_task(self._run_sync_socket(sync_socket)))
        return True

    def shutdown(self):
        for task in self.tasks:
            task.cancel()

    # ===============================================================================

    async def _run_sync_socket(self, sync_socket):
        try:
            print("Running task")
            syncer = Syncer(sync_socket)
            await syncer.run()
        except Exception as e:
            print("Error while running Syncer")
            self._handle_socket_error(sync_socket, e)
            return

    def _init_TCP_client_socket(self, port: int, address: str = ""):
        new_socket = self._init_socket(port, address)
        if new_socket is None:
            return

        new_socket = self._bind_socket(new_socket, port, address)
        if new_socket is None:
            return

        print("Listening on: " + str(address) + ":" + str(port))
        return new_socket

    def _init_socket(self, port: int, address: str):
        print("Creating TCP socket on: ", address, ":", str(port))
        try:
            return socket.socket()
        except Exception as e:
            print("Error while creating TCP socket on: ", address, ":", str(port))
            print(str(e))
            return None

    def _bind_socket(self, socket_to_bind: socket.socket, port: int, address: str):
        print("Binding to socket... " + str(address) + ":" + str(port))
        try:
            socket_to_bind.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            socket_to_bind.bind((address, port))
            socket_to_bind.listen(1)
            return socket_to_bind
        except Exception as e:
            print("Error while binding socket on address: ", address, ":", port)
            self._handle_socket_error(socket_to_bind, e)
            return None

    def _handle_socket_error(self, failed_socket, error):
        print(str(error))

        if failed_socket is not None:
            failed_socket.close()

