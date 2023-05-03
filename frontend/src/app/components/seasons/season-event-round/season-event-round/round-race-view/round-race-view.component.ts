import { Component, Input } from '@angular/core';
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

  constructor(private utils: UtilsService) {}
}
