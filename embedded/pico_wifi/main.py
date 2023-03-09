from ServerManager import ServerManager
from secrets import WIFI_SSID, WIFI_PASSWORD
import network
import machine
from time import sleep
import uasyncio

SYNC_SERVER_PORT = 15000
EVENT_SERVER_PORT = 12000

led = machine.Pin("LED", machine.Pin.OUT)


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
    global led
    led.off()

    try:
        ip = connect()

        server_manager = ServerManager()
        sensors = []

        
        sync_server = uasyncio.create_task(server_manager.launch_sync_server(ip, SYNC_SERVER_PORT))
        event_server = uasyncio.create_task(server_manager.launch_event_server(sensors, ip, EVENT_SERVER_PORT))
        
        await sync_server
        await event_server

    except KeyboardInterrupt:
        machine.reset()


uasyncio.run(main())


