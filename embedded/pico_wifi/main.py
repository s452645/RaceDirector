from SocketManager import SocketManager
from secrets import WIFI_SSID, WIFI_PASSWORD
import network
import machine
from time import sleep
import uasyncio


def connect():
    # Connect to WLAN
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan.connect(WIFI_SSID, WIFI_PASSWORD)
    while wlan.isconnected() == False:
        print("Waiting for connection...")
        sleep(1)
    ip = wlan.ifconfig()[0]
    print(f"Connected on {ip}")
    return ip


async def main():
    try:
        ip = connect()
        socket_manager = SocketManager()
        await socket_manager.launch_sync_socket(ip)
        print("Launched")
        while True:
            await uasyncio.sleep_ms(100_000)
    except KeyboardInterrupt:
        machine.reset()

uasyncio.run(main())
