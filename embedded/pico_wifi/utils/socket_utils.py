import uasyncio
from timers.AbstractTimer import AbstractTimer


async def send(
    writer: uasyncio.StreamWriter, timer: AbstractTimer, param: str, data: str
):
    t = timer.get_current_time()

    data = f"{param}->{data}"
    writer.write(data.encode("utf8"))
    await writer.drain()

    print("Sent " + str(data))
    return t


async def recv(
    reader: uasyncio.StreamReader, timer: AbstractTimer, expected_param: str
) -> tuple[int, str]:
    request = await reader.read(1024)
    request = request.decode("utf8")
    t = timer.get_current_time()

    print("Received ", str(request))

    data = request.split("->")
    if data[0] != expected_param:
        print(f"Recv failed: expected {expected_param} param, got {data[0]}")

        raise (
            AssertionError(
                f"Recv failed: expected {expected_param} param, got {data[0]}"
            )
        )

    return t, data[1]
