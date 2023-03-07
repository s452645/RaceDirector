import uasyncio
from Timer import Timer


class Syncer:
    def __init__(self, sync_socket):
        self.sync_socket = sync_socket
        self.timer = Timer()

    async def run(self):
        print("Waiting for master clock to connect...")
        self.client = self.sync_socket.accept()[0]
        print("Master clock connected: ", self.client)

        while True:
            request = self.client.recv(1024).decode("utf8")

            if request == "sync":
                print("Sync started...")
                await uasyncio.wait_for_ms(self._sync_clock(), 1500)
            else:
                print("Unknown command: ", str(request))

    # ===============================================================================

    async def _sync_clock(self):
        self._send("ready")

        t1 = 0
        t2 = 0
        t3 = 0
        t4 = 0

        try:
            t1, t2 = self._sync_packet()
            t3, t4 = self._delay_packet()
        except AssertionError:
            print("Current sync aborted. Waiting for next.")
            return

        print("t1: " + str(t1))
        print("t2: " + str(t2))
        print("t3: " + str(t3))
        print("t4: " + str(t4))

        t1 = int(t1)
        t2 = int(t2)
        t3 = int(t3)
        t4 = int(t4)

        self.timer.calculate_offset(t1, t2, t3, t4)
        self._send("{}")

    def _sync_packet(self):
        t2, _ = self._recv("[1]")
        _, t1 = self._recv("[2]")

        return t1, t2

    def _delay_packet(self):
        t3 = self._send("-")
        _, t4 = self._recv("[3]")

        return t3, t4

    def _recv(self, expected_param):
        request = self.client.recv(1024)
        t = self._get_time()

        request = str(request.decode("utf-8"))
        print("Received ", str(request))

        data = request.split("->")
        if data[0] != expected_param:
            raise (
                AssertionError(
                    f"Recv failed: expected {expected_param} param, got {data[0]}"
                )
            )

        return t, data[1]

    def _send(self, data):
        t = self._get_time()
        self.client.send(data.encode("utf8"))
        print("Sent " + str(data))
        return t

    def _get_time(self):
        return self.timer.get_current_time()
