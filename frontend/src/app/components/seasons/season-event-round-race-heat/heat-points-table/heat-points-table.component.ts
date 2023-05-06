import { Component, Input } from '@angular/core';
import { SeasonEventRoundRaceHeatResultDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';

@Component({
  selector: 'app-heat-points-table',
  templateUrl: './heat-points-table.component.html',
  styleUrls: ['./heat-points-table.component.css'],
})
export class HeatPointsTableComponent {
  @Input()
  public heatResults: SeasonEventRoundRaceHeatResultDto[] = [];
}
