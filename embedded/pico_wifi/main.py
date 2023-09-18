from ServerManager import ServerManager
from secrets import PICO_WIFI_SSID, PICO_WIFI_PASSWORD
import network
import machine
from time import sleep
import uasyncio

SERVER_PORT = 15000

MODE_ACCESS_POINT = network.AP_IF
MODE_CLIENT = network.STA_IF

BOARD_MODE = MODE_CLIENT
led = machine.Pin("LED", machine.Pin.OUT)


def connect():
    wlan = network.WLAN(BOARD_MODE)

    if BOARD_MODE == MODE_ACCESS_POINT:
        wlan.config(essid=PICO_WIFI_SSID, password=PICO_WIFI_PASSWORD, channel=10)
        wlan.active(True)

        while not wlan.isconnected():
            print("Starting Access Point...")
            sleep(1)

    else:
        wlan.active(True)
        wlan.connect(PICO_WIFI_SSID, PICO_WIFI_PASSWORD)
        while wlan.isconnected() == False:
            print("Waiting for connection...")
            sleep(1)

    ip = wlan.ifconfig()[0]
    print(f"Connected on {ip}")
    return ip


async def main():
    global led
    led.off()

    try:
        ip = connect()

        server_manager = ServerManager()
        server = await server_manager.start_server(ip, SERVER_PORT)

        async with server:
            while True:
                await uasyncio.sleep(60)

    except KeyboardInterrupt:
        machine.reset()


uasyncio.run(main())
