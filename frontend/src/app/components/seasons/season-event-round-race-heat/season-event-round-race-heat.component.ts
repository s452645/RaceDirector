import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { SeasonEventRoundRacesService } from 'src/app/services/seasons/events/rounds/races/season-event-round-races.service';
import {
  SeasonEventRoundRaceDto,
  SeasonEventRoundRaceHeatDto,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event-round-race-heat',
  templateUrl: './season-event-round-race-heat.component.html',
  styleUrls: ['./season-event-round-race-heat.component.css'],
})
export class SeasonEventRoundRaceHeatComponent implements OnInit, OnDestroy {
  private roundIdNullable: string | null = null;
  private raceIdNullable: string | null = null;
  private heatIdNullable: string | null = null;

  private raceDtoNullable: SeasonEventRoundRaceDto | null = null;
  private heatDtoNullable: SeasonEventRoundRaceHeatDto | null = null;

  private subscription = new Subscription();

  nextHeatAvailable = false;
  previousHeatAvailable = false;

  get roundId(): string {
    return this.utils.getNullableOrThrow(this.roundIdNullable);
  }

  get raceId(): string {
    return this.utils.getNullableOrThrow(this.raceIdNullable);
  }

  get raceDto(): SeasonEventRoundRaceDto {
    return this.utils.getNullableOrThrow(this.raceDtoNullable);
  }

  get heatId(): string {
    return this.utils.getNullableOrThrow(this.heatIdNullable);
  }

  get heatDto(): SeasonEventRoundRaceHeatDto {
    return this.utils.getNullableOrThrow(this.heatDtoNullable);
  }

  constructor(
    private route: ActivatedRoute,
    private racesService: SeasonEventRoundRacesService,
    private utils: UtilsService
  ) {
    this.roundIdNullable = this.route.snapshot.paramMap.get('roundId');
    this.raceIdNullable = this.route.snapshot.paramMap.get('raceId');

    this.heatIdNullable = this.route.snapshot.queryParamMap.get('startHeatId');
  }

  ngOnInit(): void {
    this.refreshData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  updateHeat(newHeatId: string): void {
    this.heatIdNullable = newHeatId;

    // TODO: add endpoint to fetch single heat
    this.refreshData();
  }

  goToNextHeat(): void {
    const currentIdx = this.raceDto.heats.findIndex(h => h.id === this.heatId);
    if (this.raceDto.heats.length <= currentIdx + 1) {
      return;
    }

    const nextHeatId = this.raceDto.heats.at(currentIdx + 1)?.id;
    this.updateHeat(nextHeatId ?? '');
  }

  goToPreviousHeat(): void {
    const currentIdx = this.raceDto.heats.findIndex(h => h.id === this.heatId);
    if (currentIdx === 0) {
      return;
    }

    const previousHeatId = this.raceDto.heats.at(currentIdx - 1)?.id;
    this.updateHeat(previousHeatId ?? '');
  }

  private refreshData(): void {
    const s = this.racesService
      .getRace(this.roundId, this.raceId)
      .subscribe(race => {
        this.raceDtoNullable = race;
        this.sortHeats();

        this.heatDtoNullable =
          race.heats.find(heat => heat.id === this.heatId) ?? null;

        this.previousHeatAvailable = false;
        this.nextHeatAvailable = false;

        if (this.heatDto.order !== 0) {
          this.previousHeatAvailable = true;
        }

        if (this.heatDto.order < this.raceDto.heats.length - 1) {
          this.nextHeatAvailable = true;
        }
      });

    this.subscription.add(s);
  }

  private sortHeats(): void {
    this.raceDto.heats.sort((a, b) => a.order - b.order);
  }
}
