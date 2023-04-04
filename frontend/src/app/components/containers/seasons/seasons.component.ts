import { Component, OnDestroy, OnInit } from '@angular/core';
import { SeasonDto } from 'src/app/services/seasons/seasons.service';
import { BaseContainerComponent } from '../base-container/base-container.component';

@Component({
  selector: 'app-seasons',
  templateUrl: './seasons.component.html',
  styleUrls: ['./seasons.component.css'],
})
export default class SeasonsComponent
  extends BaseContainerComponent
  implements OnInit, OnDestroy
{
  public seasons: SeasonDto[] = [];

  public newSeasonName: string | undefined;
  public newSeasonStartDate: Date | undefined;
  public newSeasonEndDate: Date | undefined;

  override ngOnInit(): void {
    super.ngOnInit();
    this.refreshData();
  }

  override ngOnDestroy(): void {
    super.ngOnDestroy();
  }

  public handleAddSeason(): void {
    const newSeason = this.validateNewSeasonData();

    this.subscription.add(
      this.seasonsService
        .addSeason(newSeason)
        .subscribe(() => this.refreshData())
    );
  }

  public handleEnterSeason(seasonId: string): void {
    this.router.navigate([seasonId], { relativeTo: this.route });
  }

  public handleDeleteSeason(seasonId: string): void {
    this.subscription.add(
      this.seasonsService
        .deleteSeason(seasonId)
        .subscribe(() => this.refreshData())
    );
  }

  private validateNewSeasonData(): SeasonDto {
    if (
      this.newSeasonName &&
      this.newSeasonStartDate &&
      this.newSeasonEndDate
    ) {
      return new SeasonDto(
        this.newSeasonName,
        this.convertToUTCDate(this.newSeasonStartDate),
        this.convertToUTCDate(this.newSeasonEndDate)
      );
    }

    this.toastMessageService.createToastMessage({
      severity: 'error',
      summary: 'Adding new season failed',
      detail: 'There is some missing data',
    });

    // TODO
    throw new Error();
  }

  private refreshData(): void {
    const s = this.seasonsService
      .getSeasons()
      .subscribe(seasons => (this.seasons = seasons));

    this.subscription.add(s);
  }

  private convertToUTCDate(localDate: Date): Date {
    const day = localDate.getDate();
    const month = localDate.getMonth();
    const year = localDate.getFullYear();

    return new Date(Date.UTC(year, month, day));
  }
}
