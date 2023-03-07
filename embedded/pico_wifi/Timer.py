import time


class Timer:
    def __init__(self):
        self.previous_offsets = []
        self.time_offset = 0
        self.last_sync = self.get_current_time()

    def calculate_offset(self, t1, t2, t3, t4):
        print("Timer init")
        MS_diff = t2 - t1
        SM_diff = t4 - t3
        print("DDD")

        offset = int((MS_diff - SM_diff) / 2)
        previous_offsets_count = len(self.previous_offsets)
        print("Current offset was ", str(offset))

        if abs(offset) > 20:
            if previous_offsets_count > 5:
                print("Assuming it's not representative. No time adjustments.")
                return

            self.time_offset = self.time_offset + offset
            print("New offset is ", str(self.time_offset), "\n")
            return

        # TODO: enter sus mode (request more syncs)
        # if next are also sus, display a warning on frontend
        if abs(offset) > 5:
            if previous_offsets_count == 10:
                avg = sum(self.previous_offsets) / previous_offsets_count
                is_avg_trustworthy = abs(avg) < 3

                if is_avg_trustworthy:
                    print(
                        "Suspicious. Avg probably could be trusted more. No time adjustments."
                    )
                    return

        self.previous_offsets.append((offset))

        offsets_count = len(self.previous_offsets)
        if offsets_count > 10:
            self.previous_offsets.pop(0)
            offsets_count -= 1

        avg_offset = int(sum(self.previous_offsets) / offsets_count)
        print("Previous offsets: ", self.previous_offsets)
        print("Average offset: ", avg_offset)

        self.time_offset = self.time_offset + avg_offset
        print("New offset is ", str(self.time_offset), "\n")

    def get_current_time(self):
        return time.ticks_ms() - self.time_offset
