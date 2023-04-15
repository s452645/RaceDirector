import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
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
import { NewSeasonEventFormComponent } from '../season-event/new-season-event-form/new-season-event-form.component';

@Component({
  selector: 'app-season-details',
  templateUrl: './season-details.component.html',
  styleUrls: ['./season-details.component.css'],
  providers: [ConfirmationService],
})
export class SeasonDetailsComponent implements OnInit, OnDestroy {
  @ViewChild(NewSeasonEventFormComponent, { static: true })
  private newEventFormCmp!: NewSeasonEventFormComponent;

  public seasonId: string | null = null;
  public season: SeasonDto | null = null;
  public seasonEvents: SeasonEventDto[] = [];

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
    this.seasonId = this.route.snapshot.paramMap.get('id');

    if (this.seasonId === null) {
      console.error('Could not initialize view. Season ID cannot be empty.');
      return;
    }

    this.subscription.add(this.refreshData());
    this.subscription.add(
      this.seasonService
        .getSeasonById(this.utils.getNullableOrThrow(this.seasonId))
        .subscribe(season => {
          this.routeTitleService.setRouteTitle(season.name);
          this.season = season;
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

  public handleDeleteSeasonEvent(event: Event, seasonEventId: string): void {
    this.confirmationService.confirm({
      target: event.target ?? undefined,
      message: 'Are you sure that you want to proceed?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.seasonService
          .deleteSeasonEvent(
            this.utils.getNullableOrThrow(this.seasonId),
            seasonEventId
          )
          .subscribe(() => this.refreshData());
      },
    });
  }

  public handleOpenNewEventForm(): void {
    this.newEventDialogVisible = true;
  }

  public handleAddNewSeasonEvent(seasonEvent: SeasonEventDto): void {
    this.subscription.add(
      this.seasonService
        .addSeasonEvent(
          this.utils.getNullableOrThrow(this.seasonId),
          seasonEvent
        )
        .subscribe(() => {
          this.newEventDialogVisible = false;
          this.newEventFormCmp.refreshForm();
          this.subscription.add(this.refreshData());
        })
    );
  }

  private refreshData(): void {
    this.isListLoading = true;

    const s = this.seasonService
      .getSeasonEvents(this.utils.getNullableOrThrow(this.seasonId))
      .subscribe(seasonEvents => {
        this.isListLoading = false;
        this.seasonEvents = seasonEvents;
      });

    this.subscription.add(s);
  }
}
