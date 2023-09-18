import sys
import machine 
import utime as time
from machine import Pin

track_A_pin = Pin(11, Pin.IN, Pin.PULL_UP)

mode_race = False
finishes = 0
winner_timestamp = 0

timestamps = list()

FULL_POINTS_ADVANTAGE_SECONDS = 5

POSITION_POINTS = {
    1: 8,
    2: 5,
    3: 3,
    4: 1
}


WHITE = "\u001b[37m"
BLACK = "\u001b[30m"
YELLOW = "\u001b[33;1m"
PURPLE = "\u001b[35;1m"
GREEN = "\u001b[32;1m"
RESET = "\u001b[0m"

DARK_GREEN = "\u001b[32m"
DARK_YELLOW = "\u001b[31;1m"
DARK_PURPLE = "\u001b[35;1m"

BG_WHITE = "\u001b[40;1m"


def display_result():
    global winner_timestamp
    global finishes
    global mode_race
    
    finishes += 1
    timestamp = 0
    track_name = ""
    
    timestamp = timestamps[-1]
    
    gap_to_winner = 0
    if finishes > 1:
        gap_to_winner = (timestamp - winner_timestamp) / 1_000_000
    else:
        winner_timestamp = timestamp
    
    gap_to_winner_rounded = round(gap_to_winner, 3)
    print(str(finishes) + ". " + track_name, end="")
    
    if finishes > 1:
        print("   + ", end="")
        print('{:.3f}'.format(gap_to_winner_rounded), end="")
        print(" s")
        
    else:
        print()
    
    
def track_A_beam_change(pin):
    global timestamps
    
    if mode_race:
        broken = pin.value() == 0
        if broken:
            timestamps.append(time.ticks_us())
            display_result()


def get_advantage_points(my_place, other_place, diff):
    global POSITIONS
    global FULL_POINTS_ADVANTAGE_SECONDS

    multiplier = POSTIONS[my_place] - POSITIONS[other_place]
    percentage = max((diff / 1_000_000) / FULL_POINTS_ADVANTAGE_SECONDS, 1)
    
    return multiplier * percentage
        


track_A_pin.irq(handler = track_A_beam_change, trigger = Pin.IRQ_FALLING | Pin.IRQ_RISING)

while True:
    if not mode_race:
        print("Choose next action:")
        print("[1] - Enter race mode")
        print("[2] - Exit")
        answer = input()
        
        if answer == "1":
            timestamps = list()
            finishes = 0
            winner_timestamp = 0
            
            mode_race = True
            print(GREEN, end="")
            print("Race mode: on", end="")
            print(RESET)
            
        elif answer == "2":
            break
            
        
    if mode_race:
        print("Press any key to stop")
        _ = input()
        
        results_dict = {
            '#1': timestamps[0],
            '#2': timestamps[1],
            '#3': timestamps[2],
            '#4': timestamps[3],
        }
        
        results_sorted = sorted(results_dict.items(), key=lambda x:x[1])
        
        current_place = 1
        for track, timestamp in results_sorted:
            position_points = POSITIONS[current_place]
            advantage_points = 0
                
            if current_place != 4:
                current_other_place = 1
                for other_track, other_timestamp in results_sorted:
                    if other_timestamp < timestamp:
                        continue
                    
                    diff = other_timestamp - timestamp
                    advantage_points += get_advantage_points(current_place, current_other_place, diff)
                    
            
            print("\n" + track +  " -> " + position_points + " + " + advantage_points + " = " + (position_points + advantage_points)) 
        
        mode_race = False