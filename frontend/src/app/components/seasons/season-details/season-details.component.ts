import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { RouteTitleService } from 'src/app/services/route-title.service';
import {
  SeasonDto,
  SeasonEventDto,
  SeasonsService,
} from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-details',
  templateUrl: './season-details.component.html',
  styleUrls: ['./season-details.component.css'],
  providers: [ConfirmationService],
})
export class SeasonDetailsComponent implements OnInit, OnDestroy {
  public seasonIdNullable: string | null = null;
  public seasonNullable: SeasonDto | null = null;

  get seasonId(): string {
    return this.utils.getNullableOrThrow(this.seasonIdNullable);
  }

  get season(): SeasonDto {
    return this.utils.getNullableOrThrow(this.season);
  }

  public seasonEvents: SeasonEventDto[] = [];

  public chosenSeasonEvent: SeasonEventDto | undefined = undefined;

  public newEventDialogVisible = false;
  public isListLoading = false;

  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private routeTitleService: RouteTitleService,
    private seasonService: SeasonsService,
    private utils: UtilsService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('id');

    if (this.seasonIdNullable === null) {
      console.error('Could not initialize view. Season ID cannot be empty.');
      return;
    }

    this.subscription.add(this.refreshData());
    this.subscription.add(
      this.seasonService.getSeasonById(this.seasonId).subscribe(season => {
        this.routeTitleService.setRouteTitle(season.name);
        this.seasonNullable = season;
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public handleEnterSeasonEvent(seasonEventId: string): void {
    this.router.navigate([`events/${seasonEventId}`], {
      relativeTo: this.route,
    });
  }

  public handleEditSeasonEvent(seasonEventId: string): void {
    this.chosenSeasonEvent = this.seasonEvents.find(
      e => e.id === seasonEventId
    );

    this.newEventDialogVisible = true;
  }

  public handleDeleteSeasonEvent(event: Event, seasonEventId: string): void {
    this.confirmationService.confirm({
      target: event.target ?? undefined,
      message: 'Are you sure that you want to proceed?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.seasonService
          .deleteSeasonEvent(this.seasonId, seasonEventId)
          .subscribe(() => this.refreshData());
      },
    });
  }

  public handleOpenNewEventForm(): void {
    this.chosenSeasonEvent = undefined;
    this.newEventDialogVisible = true;
  }

  public handleCloseEventForm(refresh: boolean): void {
    this.chosenSeasonEvent = undefined;
    this.newEventDialogVisible = false;

    if (refresh) {
      this.subscription.add(this.refreshData());
    }
  }

  private refreshData(): void {
    this.isListLoading = true;

    const s = this.seasonService
      .getSeasonEvents(this.seasonId)
      .subscribe(seasonEvents => {
        this.isListLoading = false;
        this.seasonEvents = seasonEvents;
      });

    this.subscription.add(s);
  }
}
