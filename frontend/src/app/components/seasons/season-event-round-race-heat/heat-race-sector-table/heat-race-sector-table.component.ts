import { Component, Input } from '@angular/core';
import { RaceHeatSectorResultDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';

@Component({
  selector: 'app-heat-race-sector-table',
  templateUrl: './heat-race-sector-table.component.html',
  styleUrls: ['./heat-race-sector-table.component.css'],
})
export class HeatRaceSectorTableComponent {
  @Input()
  public sectorResults: RaceHeatSectorResultDto[] = [];
}
