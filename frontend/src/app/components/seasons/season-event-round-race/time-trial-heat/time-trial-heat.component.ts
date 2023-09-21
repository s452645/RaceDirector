import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { isNil, sum } from 'lodash';
import { Subscription } from 'rxjs';
import { ElevatorService } from 'src/app/services/hardware/elevator.service';
import {
  RaceState,
  SeasonEventRoundRaceResultData,
  SeasonEventRoundRaceResultDto,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-time-trial-heat',
  templateUrl: './time-trial-heat.component.html',
  styleUrls: ['./time-trial-heat.component.css'],
})
export class TimeTrialHeatComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  public raceResult: SeasonEventRoundRaceResultDto | null = null;

  @Output()
  public raceDataChanged: EventEmitter<SeasonEventRoundRaceResultData> =
    new EventEmitter();

  @Output()
  public raceStarted: EventEmitter<string> = new EventEmitter();

  @Output()
  public raceDNFed: EventEmitter<string> = new EventEmitter();

  public form: FormGroup = new FormGroup({});

  public timer = 0;
  public startTime = 0;
  public timerRunning = false;
  public timerRef: any;

  private subscription = new Subscription();

  public constructor(private fb: FormBuilder, private util: UtilsService) {}

  public ngOnInit(): void {
    this.refreshForm();
  }

  public ngOnChanges(_: SimpleChanges): void {
    this.refreshTimer();
    this.refreshForm();
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public onRaceStarted(): void {
    this.raceStarted.emit(this.raceResult?.id);
  }

  public onRaceDNFed(): void {
    this.raceDNFed.emit(this.raceResult?.id);
  }

  public isInactive(): boolean {
    return (
      this.raceResult?.raceState == null ||
      RaceState.Inactive === this.raceResult?.raceState
    );
  }

  private refreshForm(): void {
    this.form = this.fb.group({
      distance: [this.raceResult?.distance],
      bonusesPoints: [this.raceResult?.bonusesPoints],
    });

    const s = this.form.valueChanges.subscribe(() => {
      const resultId = this.util.getUndefinedableOrThrow(this.raceResult?.id);
      const distance = this.form.get('distance')?.value;
      const bonusesPoints = this.form.get('bonusesPoints')?.value;

      if (isNil(distance) && isNil(bonusesPoints)) {
        return;
      }

      const raceData = new SeasonEventRoundRaceResultData(
        resultId,
        distance,
        bonusesPoints
      );

      this.raceDataChanged.emit(raceData);
    });

    this.subscription.add(s);
  }

  private refreshTimer(): void {
    this.startTime = Date.now() / 1000 - sum(this.raceResult?.sectorTimes) || 0;
    this.timerRunning = this.raceResult?.raceState === RaceState.Active;
    this.timer = Date.now() / 1000 - this.startTime;

    if (this.timerRef) {
      clearInterval(this.timerRef);
      this.timerRef = null;
    }

    if (this.timerRunning) {
      this.timerRef = setInterval(() => {
        this.timer = Date.now() / 1000 - this.startTime;
      });
    } else {
      this.timer = sum(this.raceResult?.sectorTimes) || 0;
      clearInterval(this.timerRef);
    }
  }

  public getHeaderClass(): string {
    const state = this.raceResult?.raceState;

    if (state == RaceState.Pending) {
      return 'text-yellow-400';
    } else if (state == RaceState.Active) {
      return 'text-green-400';
    } else {
      return 'text-secondary';
    }
  }
}
