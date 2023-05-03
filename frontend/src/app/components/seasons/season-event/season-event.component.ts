import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, mergeMap, of } from 'rxjs';
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

@Component({
  selector: 'app-season-event',
  templateUrl: './season-event.component.html',
  styleUrls: ['./season-event.component.css'],
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

  handleEditRound(roundId: string) {
    this.chosenRound = this.utils.getUndefinedableOrThrow(
      this.rounds.find(r => r.id === roundId)
    );

    this.nextRoundParticipantsCount = this.chosenRound.participantsCount;
    this.nextRoundOrder = this.chosenRound.order;

    this.isRoundFormOpen = true;
  }

  handleDeleteRound(roundId: string) {
    this.subscription.add(
      this.roundsService
        .deleteRound(this.seasonEventId, roundId)
        .subscribe(() => {
          this.refreshData();
        })
    );
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
