import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SeasonEventRoundRacesService } from 'src/app/services/seasons/events/rounds/races/season-event-round-races.service';
import { SeasonEventRoundRaceDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event-round-race',
  templateUrl: './season-event-round-race.component.html',
  styleUrls: ['./season-event-round-race.component.css'],
})
export class SeasonEventRoundRaceComponent implements OnInit, OnDestroy {
  private seasonIdNullable: string | null = null;
  private seasonEventIdNullable: string | null = null;
  private roundIdNullable: string | null = null;
  private raceIdNullable: string | null = null;

  private raceDtoNullable: SeasonEventRoundRaceDto | null = null;

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

  get raceDto(): SeasonEventRoundRaceDto {
    return this.utils.getNullableOrThrow(this.raceDtoNullable);
  }

  constructor(
    private route: ActivatedRoute,
    private utils: UtilsService,
    private raceService: SeasonEventRoundRacesService
  ) {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventIdNullable = this.route.snapshot.paramMap.get('eventId');
    this.roundIdNullable = this.route.snapshot.paramMap.get('roundId');
    this.raceIdNullable = this.route.snapshot.paramMap.get('raceId');
  }

  ngOnInit(): void {
    this.refreshData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private refreshData(): void {
    this.subscription.add(
      this.raceService.getRace(this.roundId, this.raceId).subscribe(race => {
        this.raceDtoNullable = race;
        this.sortHeats();
      })
    );
  }

  private sortHeats(): void {
    this.raceDto.heats.sort((a, b) => a.order - b.order);
  }
}
