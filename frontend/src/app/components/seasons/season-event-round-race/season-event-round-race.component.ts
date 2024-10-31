import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { add, isEmpty, isNil, sum } from 'lodash';
import { noop, Subject, Subscription } from 'rxjs';
import { ElevatorService } from 'src/app/services/hardware/elevator.service';
import { SeasonEventRoundRacesService } from 'src/app/services/seasons/events/rounds/races/season-event-round-races.service';
import {
  SeasonEventRoundRaceDto,
  SeasonEventRoundRaceResultData,
  SeasonEventRoundRaceResultDto,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';
import { WebSocketService } from 'src/app/services/websocket.service';

@Component({
  selector: 'app-season-event-round-race',
  templateUrl: './season-event-round-race.component.html',
  styleUrls: ['./season-event-round-race.component.css'],
})
export class SeasonEventRoundRaceComponent implements OnInit, OnDestroy {
  public availableBonuses: number[] = [];
  public activeRaceResults: SeasonEventRoundRaceResultDto[] = [];

  public next: SeasonEventRoundRaceResultDto | undefined;
  public nextnext: SeasonEventRoundRaceResultDto | undefined;

  private roundIdNullable: string | null = null;
  private raceIdNullable: string | null = null;
  private raceDtoNullable: SeasonEventRoundRaceDto | null = null;

  private webSocketMessages: Subject<string> = new Subject();
  private subscription = new Subscription();

  get roundId(): string {
    return this.utils.getNullableOrThrow(this.roundIdNullable);
  }

  get raceId(): string {
    return this.utils.getNullableOrThrow(this.raceIdNullable);
  }

  get raceDto(): SeasonEventRoundRaceDto {
    return this.utils.getNullableOrThrow(this.raceDtoNullable);
  }

  constructor(
    private raceService: SeasonEventRoundRacesService,
    private route: ActivatedRoute,
    private utils: UtilsService,
    private webSocketService: WebSocketService,
    private elevatorService: ElevatorService
  ) {
    this.roundIdNullable = this.route.snapshot.paramMap.get('roundId');
    this.raceIdNullable = this.route.snapshot.paramMap.get('raceId');
  }

  public ngOnInit(): void {
    this.registerWebSocket();
    this.refreshRace();
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public getTotalTime(sectorTimes: number[]): string {
    return sum(sectorTimes).toFixed(3);
  }

  public getTotalBonuses(bonuses: number[]): string {
    return sum(bonuses).toFixed(0);
  }

  public handleEnterHeat(result: SeasonEventRoundRaceResultDto): void {
    this.activeRaceResults.push(result);
    if (this.activeRaceResults.length > 3) {
      this.activeRaceResults.shift();
    }
  }

  public handleRaceDataChange(data: SeasonEventRoundRaceResultData): void {
    this.raceService.saveDistanceAndBonuses(this.raceId, data).subscribe(() => {
      return null;
    });
  }

  public handleRaceStarted(raceResultId: string): void {
    const s = this.raceService
      .startRace(this.raceId, raceResultId)
      .subscribe(() => {
        return null;
      });
    this.subscription.add(s);
  }

  public handleRaceDNF(raceResultId: string): void {
    const s = this.raceService
      .DNFrace(this.raceId, raceResultId)
      .subscribe(() => {
        return null;
      });

    this.subscription.add(s);
  }

  private refreshRace(): void {
    const s1 = this.raceService
      .getRace(this.roundId, this.raceId)
      .subscribe(race => {
        this.raceDtoNullable = race;
        this.updateActiveRaceResult();
        this.sortRaceResults();

        if (isNil(this.next)) {
          // this.next = this.raceDto.firstResultFromNextRace;
        }

        if (isNil(this.nextnext)) {
          // this.nextnext = this.raceDto.secondResultFromNextRace;
        }
      });

    this.subscription.add(s1);
  }

  private updateActiveRaceResult(
    additional: SeasonEventRoundRaceResultDto[] = []
  ): void {
    if (isEmpty(this.activeRaceResults)) {
      return;
    }

    const newResults = this.activeRaceResults
      .map(result => {
        const id = result.id;
        return [
          ...additional,
          this.next,
          this.nextnext,
          ...this.raceDto.results,
        ].find(r => r?.id === id);
      })
      .filter(result => !isNil(result));

    this.activeRaceResults = [];
    newResults.forEach(result => {
      if (result !== undefined) {
        this.activeRaceResults.push(result);
      }
    });
  }

  private sortRaceResults(): void {
    this.raceDto.results.sort((a, b) => {
      const posA =
        a.position == null || a.position == 0 ? Infinity : a.position;

      const posB =
        b.position == null || b.position == 0 ? Infinity : b.position;

      return posA - posB;
    });
  }

  private registerWebSocket(): void {
    this.webSocketMessages =
      this.webSocketService.createSyncWebSocket<string>('race');

    this.subscription.add(
      this.webSocketMessages.subscribe(message =>
        this.handleWebSocketMessage(message)
      )
    );
  }

  private handleWebSocketMessage(message: string): void {
    try {
      const newRaceDto = SeasonEventRoundRaceDto.fromWebSocketPayload(
        JSON.parse(message)
      );

      if (newRaceDto.id === this.raceId) {
        this.raceDtoNullable = newRaceDto;
        this.updateActiveRaceResult();
        this.sortRaceResults();
      } else {
        this.updateActiveRaceResult(newRaceDto.results);
      }
    } catch (err) {
      console.error(err);
    }
  }

  public sendCommand(message: string): void {
    this.elevatorService
      .sendCommand(message)
      .subscribe(response => console.log(response));
  }

  public sendGateCommand(gateNumber: number): void {
    this.elevatorService.sendGateCommmand(gateNumber).subscribe(noop);
  }
}
