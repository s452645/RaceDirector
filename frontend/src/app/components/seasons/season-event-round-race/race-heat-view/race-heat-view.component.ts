import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SelectItem } from 'primeng/api';
import { Subject, Subscription } from 'rxjs';
import { SeasonEventRoundRacesService } from 'src/app/services/seasons/events/rounds/races/season-event-round-races.service';
import { SeasonEventRoundRaceHeatDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { SeasonsService } from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';
import {
  WebSocketMsg,
  WebSocketService,
} from 'src/app/services/websocket.service';

@Component({
  selector: 'app-race-heat-view',
  templateUrl: './race-heat-view.component.html',
  styleUrls: ['./race-heat-view.component.css'],
})
export class RaceHeatViewComponent implements OnInit, OnDestroy {
  bonusesOptions: SelectItem[] = [];

  distance = 0;
  selectedBonuses: number[] = [];

  private seasonIdNullable: string | null = null;
  private seasonEventIdNullable: string | null = null;
  private roundIdNullable: string | null = null;
  private raceIdNullable: string | null = null;
  private heatIdNullable: string | null = null;

  private heatDtoNullable: SeasonEventRoundRaceHeatDto | null = null;

  private newHeatStateMessages: Subject<string> = new Subject();

  private subscription = new Subscription();

  get seasonId(): string {
    return this.utils.getNullableOrThrow(this.seasonIdNullable);
  }

  get seasonEventId(): string {
    return this.utils.getNullableOrThrow(this.seasonEventIdNullable);
  }

  get roundId(): string {
    return this.utils.getNullableOrThrow(this.roundIdNullable);
  }

  get raceId(): string {
    return this.utils.getNullableOrThrow(this.raceIdNullable);
  }

  get heatId(): string {
    return this.utils.getNullableOrThrow(this.heatIdNullable);
  }

  get heatDto(): SeasonEventRoundRaceHeatDto {
    return this.utils.getNullableOrThrow(this.heatDtoNullable);
  }

  constructor(
    private route: ActivatedRoute,
    private utils: UtilsService,
    private raceService: SeasonEventRoundRacesService,
    private eventService: SeasonsService,
    private webSocketService: WebSocketService
  ) {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventIdNullable = this.route.snapshot.paramMap.get('eventId');
    this.roundIdNullable = this.route.snapshot.paramMap.get('roundId');
    this.raceIdNullable = this.route.snapshot.paramMap.get('raceId');
    this.heatIdNullable = this.route.snapshot.paramMap.get('heatId');
  }

  ngOnInit(): void {
    this.refreshData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  startHeat(): void {
    this.subscription.add(
      this.raceService.beginHeat(this.heatId).subscribe(() => {
        this.newHeatStateMessages =
          this.webSocketService.createSyncWebSocket<string>('heat');

        this.subscription.add(
          this.newHeatStateMessages.subscribe(message =>
            this.handleNewHeatStateMessage(message)
          )
        );
      })
    );
  }

  save(): void {
    this.subscription.add(
      this.raceService
        .saveDistanceAndBonuses(this.distance, this.selectedBonuses)
        .subscribe(() => this.refreshData())
    );
  }

  endHeat(): void {
    this.subscription.add(
      this.raceService
        .endHeat()
        .subscribe(() => this.refreshData())
        .add(() => (this.newHeatStateMessages = new Subject()))
    );
  }

  goToPrevious(): void {
    console.warn('Go to previous');
  }

  goToNext(): void {
    console.warn('Go to next');
  }

  displaySectorTimes(sectorTimes: number[]): string {
    return sectorTimes.map(time => time.toFixed(3)).join(' - ');
  }

  displayBonuses(bonuses: number[]): string {
    return bonuses.join(' • ');
  }

  private handleNewHeatStateMessage(message: string): void {
    try {
      this.heatDtoNullable = JSON.parse(message);
    } catch (err) {
      console.error(err);
    }
  }

  private refreshData(): void {
    this.subscription.add(
      this.raceService.getRace(this.roundId, this.raceId).subscribe(race => {
        const heat = race.heats.find(h => h.id === this.heatId);
        this.heatDtoNullable = heat ?? null;
      })
    );

    this.subscription.add(
      this.eventService
        .getSeasonEventById(this.seasonId, this.seasonEventId)
        .subscribe(seasonEvent => {
          const bonuses = seasonEvent.scoreRules?.availableBonuses;

          if (!bonuses) {
            return;
          }

          this.bonusesOptions = bonuses.map(bonus => {
            return {
              label: bonus.toLocaleString(),
              value: bonus,
            };
          });
        })
    );
  }
}
