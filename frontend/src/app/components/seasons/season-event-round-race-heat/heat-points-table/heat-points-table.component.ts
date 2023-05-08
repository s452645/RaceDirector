import { Component, Input } from '@angular/core';
import { RaceHeatResultDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-heat-points-table',
  templateUrl: './heat-points-table.component.html',
  styleUrls: ['./heat-points-table.component.css'],
})
export class HeatPointsTableComponent {
  @Input()
  public heatResults: RaceHeatResultDto[] = [];

  @Input()
  public availableBonuses: number[] = [];

  constructor(private utils: UtilsService) {}

  clonedResults: { [s: string]: RaceHeatResultDto } = {};

  onRowEditInit(result: RaceHeatResultDto) {
    this.clonedResults[this.utils.getUndefinedableOrThrow(result.id)] = {
      ...result,
    };
  }

  onRowEditSave(result: RaceHeatResultDto) {
    delete this.clonedResults[this.utils.getUndefinedableOrThrow(result.id)];

    // TODO: update via request
  }

  onRowEditCancel(result: RaceHeatResultDto, index: number) {
    const resultId = this.utils.getUndefinedableOrThrow(result.id);

    this.heatResults[index] = this.clonedResults[resultId];
    delete this.clonedResults[resultId];
  }
}
