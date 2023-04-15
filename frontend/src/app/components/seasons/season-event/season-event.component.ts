import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, first, map, mergeMap, of } from 'rxjs';
import {
  CircuitDto,
  CircuitService,
} from 'src/app/services/seasons/events/circuit.service';
import { RouteTitleService } from 'src/app/services/route-title.service';
import {
  SeasonEventDto,
  SeasonEventScoreRulesDto,
  SeasonsService,
} from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';
import { CircuitFormComponent } from '../../circuit/circuit-form/circuit-form.component';
import { SeasonEventScoreRulesFormComponent } from './season-event-score-rules-form/season-event-score-rules-form.component';
import {
  SeasonEventRoundDto,
  SeasonEventRoundsService,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { RoundFormComponent } from '../season-event-round/round-form/round-form.component';

@Component({
  selector: 'app-season-event',
  templateUrl: './season-event.component.html',
  styleUrls: ['./season-event.component.css'],
})
export class SeasonEventComponent implements OnInit, OnDestroy {
  @ViewChild(CircuitFormComponent, { static: true })
  private circuitFormCmp!: CircuitFormComponent;

  @ViewChild(SeasonEventScoreRulesFormComponent, { static: true })
  private scoreRulesFormCmp!: SeasonEventScoreRulesFormComponent;

  @ViewChild(RoundFormComponent, { static: true })
  private newRoundFormCmp!: RoundFormComponent;

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
  isNewRoundFormOpen = false;

  nextRoundParticipantsCount = 0;
  nextRoundOrder = 0;

  rounds: SeasonEventRoundDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private routeTitleService: RouteTitleService,
    private seasonsService: SeasonsService,
    private utils: UtilsService,
    private circuitService: CircuitService,
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
    this.circuitFormCmp.refreshForm();
    this.isCircuitFormOpen = true;
  }

  handleOpenScoreRulesForm(): void {
    this.scoreRulesFormCmp.refreshForm();
    this.isScoreRulesFormOpen = true;
  }

  handleOpenNewRoundForm(): void {
    this.subscription.add(
      this.getNewRoundParticipantsCount().subscribe(count => {
        this.nextRoundParticipantsCount = count;

        this.newRoundFormCmp.refreshForm();
        this.isNewRoundFormOpen = true;
      })
    );
  }

  handleSubmittedCircuit(circuit: CircuitDto): void {
    this.subscription.add(
      this.circuitService
        .addCircuit(this.seasonEventId, circuit)
        .subscribe(() => this.refreshData())
        .add(() => {
          this.circuitFormCmp.isSubmitButtonLoading = false;
          this.isCircuitFormOpen = false;
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

  handleSubmittedRound(round: SeasonEventRoundDto): void {
    this.subscription.add(
      this.roundsService
        .addRound(this.seasonEventId, round)
        .subscribe(() => this.refreshData())
        .add(() => {
          this.newRoundFormCmp.isSubmitButtonLoading = false;
          this.isNewRoundFormOpen = false;
        })
    );
  }

  goToFirstRound(): void {
    this.subscription.add(
      this.roundsService.getRounds(this.seasonEventId).subscribe(rounds => {
        const firstRound = rounds.find(r => r.order === 0);

        if (firstRound === undefined) {
          return;
        }

        this.router.navigate([`rounds/${firstRound?.id}`], {
          relativeTo: this.route,
        });
      })
    );
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
