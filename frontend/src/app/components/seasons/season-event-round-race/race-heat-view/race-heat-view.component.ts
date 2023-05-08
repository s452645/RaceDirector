import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SeasonEventRoundRaceHeatDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-race-heat-view',
  templateUrl: './race-heat-view.component.html',
  styleUrls: ['./race-heat-view.component.css'],
})
export class RaceHeatViewComponent {
  @Input()
  public heatDtoNullable: SeasonEventRoundRaceHeatDto | null = null;

  get heatDto(): SeasonEventRoundRaceHeatDto {
    return this.utils.getNullableOrThrow(this.heatDtoNullable);
  }

  constructor(
    private utils: UtilsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  goToHeat(heatId: string | undefined): void {
    if (!heatId) {
      return;
    }

    this.router.navigate([`heats`], {
      relativeTo: this.route,
      queryParams: { startHeatId: heatId },
    });
  }
}
