import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SeasonEventRoundRaceDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-round-race-view',
  templateUrl: './round-race-view.component.html',
  styleUrls: ['./round-race-view.component.css'],
})
export class RoundRaceViewComponent {
  @Input()
  public raceNullable: SeasonEventRoundRaceDto | null = null;

  get race(): SeasonEventRoundRaceDto {
    return this.utils.getNullableOrThrow(this.raceNullable);
  }

  constructor(
    private utils: UtilsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  goToRace(raceId: string | undefined): void {
    if (!raceId) {
      return;
    }

    this.router.navigate([`races/${raceId}`], {
      relativeTo: this.route,
    });
  }
}
