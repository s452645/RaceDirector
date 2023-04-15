import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import {
  SeasonEventRoundDto,
  SeasonEventRoundsService,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event-round',
  templateUrl: './season-event-round.component.html',
  styleUrls: ['./season-event-round.component.css'],
})
export class SeasonEventRoundComponent implements OnInit, OnDestroy {
  private seasonIdNullable: string | null = null;
  private seasonEventIdNullable: string | null = null;
  private roundIdNullable: string | null = null;
  private roundDtoNullable: SeasonEventRoundDto | null = null;

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

  get roundDto(): SeasonEventRoundDto {
    return this.utils.getNullableOrThrow(this.roundDtoNullable);
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private utils: UtilsService,
    private roundsService: SeasonEventRoundsService
  ) {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventIdNullable = this.route.snapshot.paramMap.get('eventId');
    this.roundIdNullable = this.route.snapshot.paramMap.get('roundId');
  }

  ngOnInit(): void {
    this.refreshData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  drawRaces(): void {
    this.subscription.add(
      this.roundsService
        .drawRaces(this.roundId)
        .subscribe(() => this.refreshData())
    );
  }

  goToRace(raceId: string): void {
    this.router.navigate([`races/${raceId}`], {
      relativeTo: this.route,
    });
  }

  refreshData(): void {
    this.subscription.add(
      this.roundsService
        .getRound(this.seasonEventId, this.roundId)
        .subscribe(round => {
          this.roundDtoNullable = round;

          this.sortRaces();
        })
    );
  }

  private sortRaces(): void {
    this.roundDto.races.sort((a, b) => a.order - b.order);

    this.roundDto.races.forEach(race => {
      race.results.sort((a, b) => (a.position ?? 0) - (b.position ?? 0));
    });
  }
}
