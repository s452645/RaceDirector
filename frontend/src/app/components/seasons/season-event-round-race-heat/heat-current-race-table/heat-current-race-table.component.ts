import { Component, Input } from '@angular/core';
import { SeasonEventRoundRaceDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-heat-current-race-table',
  templateUrl: './heat-current-race-table.component.html',
  styleUrls: ['./heat-current-race-table.component.css'],
})
export class HeatCurrentRaceTableComponent {
  @Input()
  public raceDtoNullable: SeasonEventRoundRaceDto | null = null;

  get raceDto(): SeasonEventRoundRaceDto {
    return this.utils.getNullableOrThrow(this.raceDtoNullable);
  }

  constructor(private utils: UtilsService) {}
}
