import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, map, mergeMap, of } from 'rxjs';
import { RouteTitleService } from 'src/app/services/route-title.service';
import {
  SeasonEventDto,
  SeasonEventScoreRulesDto,
  SeasonsService,
} from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';
import {
  SeasonEventRoundDto,
  SeasonEventRoundsService,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { SeasonEventScoreRulesFormComponent } from './season-event-score-rules-form/season-event-score-rules-form.component';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-season-event',
  templateUrl: './season-event.component.html',
  styleUrls: ['./season-event.component.css'],
  providers: [ConfirmationService],
})
export class SeasonEventComponent implements OnInit, OnDestroy {
  @ViewChild(SeasonEventScoreRulesFormComponent, { static: true })
  private scoreRulesFormCmp!: SeasonEventScoreRulesFormComponent;

  private seasonIdNullable: string | null = null;
  private seasonEventIdNullable: string | null = null;
  private seasonEventNullable: SeasonEventDto | null = null;
  private subscription = new Subscription();

  get seasonId(): string {
    return this.utils.getNullableOrThrow(this.seasonIdNullable);
  }

  get seasonEventId(): string {
    return this.utils.getNullableOrThrow(this.seasonEventIdNullable);
  }

  get seasonEvent(): SeasonEventDto {
    return this.utils.getNullableOrThrow(this.seasonEventNullable);
  }

  isCircuitFormOpen = false;
  isScoreRulesFormOpen = false;
  isRoundFormOpen = false;

  nextRoundParticipantsCount = 0;
  nextRoundOrder = 0;

  public rounds: SeasonEventRoundDto[] = [];
  public chosenRound: SeasonEventRoundDto | undefined;

  constructor(
    private route: ActivatedRoute,
    private routeTitleService: RouteTitleService,
    private seasonsService: SeasonsService,
    private utils: UtilsService,
    private roundsService: SeasonEventRoundsService,
    private confirmationService: ConfirmationService,
    private router: Router
  ) {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventIdNullable = this.route.snapshot.paramMap.get('eventId');
  }

  ngOnInit(): void {
    this.subscription.add(this.refreshData());
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  handleOpenCircuitForm(): void {
    this.isCircuitFormOpen = true;
  }

  handleOpenScoreRulesForm(): void {
    this.scoreRulesFormCmp.refreshForm();
    this.isScoreRulesFormOpen = true;
  }

  handleOpenNewRoundForm(): void {
    this.chosenRound = undefined;

    this.subscription.add(
      this.getNewRoundParticipantsCount().subscribe(count => {
        this.nextRoundParticipantsCount = count;
        this.isRoundFormOpen = true;
      })
    );
  }

  handleEnterRound(roundId: string) {
    this.router.navigate([`rounds/${roundId}`], {
      relativeTo: this.route,
    });
  }

  handleEditRound(event: Event, roundId: string) {
    const s = this.roundsService
      .hasRoundStarted(roundId)
      .subscribe(hasStarted => {
        if (!hasStarted) {
          this.editRound(roundId);
          return;
        }

        this.displayConfirmationPopup(
          event,
          `This round has already started. Editing can result in a loss of data. Do you want to continue?`,
          () => this.editRound(roundId)
        );
      });

    this.subscription.add(s);
  }

  private editRound(roundId: string): void {
    this.chosenRound = this.utils.getUndefinedableOrThrow(
      this.rounds.find(r => r.id === roundId)
    );

    this.nextRoundParticipantsCount = this.chosenRound.participantsCount;
    this.nextRoundOrder = this.chosenRound.order;

    this.isRoundFormOpen = true;
  }

  handleDeleteRound(event: Event, roundId: string) {
    const s = this.roundsService
      .hasRoundStarted(roundId)
      .subscribe(hasStarted => {
        if (!hasStarted) {
          this.deleteRound(roundId);
          return;
        }

        this.displayConfirmationPopup(
          event,
          'This round has already started. Deleting will result in a loss of data. Do you want to continue?',
          () => this.deleteRound(roundId)
        );
      });

    this.subscription.add(s);
  }

  private deleteRound(roundId: string) {
    this.subscription.add(
      this.roundsService
        .deleteRound(this.seasonEventId, roundId)
        .subscribe(() => {
          this.refreshData();
        })
    );
  }

  private displayConfirmationPopup(
    event: Event,
    msg: string,
    onAccept: () => void
  ): void {
    this.confirmationService.confirm({
      target: event.target ?? undefined,
      message: msg,
      icon: 'pi pi-exclamation-triangle',
      accept: () => onAccept(),
    });
  }

  handleSubmittedScoreRules(scoreRules: SeasonEventScoreRulesDto): void {
    this.subscription.add(
      this.seasonsService
        .addSeasonEventScoreRules(this.seasonEventId, scoreRules)
        .subscribe(() => this.refreshData())
        .add(() => {
          this.scoreRulesFormCmp.isSubmitButtonLoading = false;
          this.isScoreRulesFormOpen = false;
        })
    );
  }

  closeDialog(refreshData: boolean) {
    this.isCircuitFormOpen = false;
    this.isRoundFormOpen = false;
    this.isScoreRulesFormOpen = false;

    if (refreshData) {
      this.refreshData();
    }
  }

  private refreshData(): void {
    this.subscription.add(
      this.seasonsService
        .getSeasonEventById(this.seasonId, this.seasonEventId)
        .subscribe(seasonEvent => {
          this.routeTitleService.setRouteTitle(seasonEvent.name);

          seasonEvent.circuit?.checkpoints.sort(
            (a, b) => a.position - b.position
          );

          this.seasonEventNullable = seasonEvent;

          // run this request in parallel
          this.subscription.add(
            this.roundsService
              .getRounds(this.seasonEventId)
              .subscribe(
                rounds =>
                  (this.rounds = rounds.sort((a, b) => a.order - b.order))
              )
          );
        })
    );
  }

  private getNewRoundParticipantsCount(): Observable<number> {
    return this.roundsService.getRounds(this.seasonEventId).pipe(
      mergeMap(rounds => {
        this.nextRoundOrder = rounds.length;

        const lastRound = rounds.sort((a, b) => a.order - b.order).at(-1);

        if (!lastRound) {
          return of(this.seasonEvent.participantsCount ?? 0);
        }

        return of(lastRound?.advancesCount ?? 0);
      })
    );
  }
}
