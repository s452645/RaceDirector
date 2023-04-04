import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SelectItem } from 'primeng/api';
import { Subscription } from 'rxjs';
import {
  DroppedCarsPositionDefinementStrategy,
  RoundPointsStrategy,
  RoundType,
  SeasonEventRoundDto,
  SeasonEventRoundRaceDto,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-round-form',
  templateUrl: './round-form.component.html',
  styleUrls: ['./round-form.component.css'],
})
export class RoundFormComponent implements OnInit, OnDestroy {
  @Input()
  participantsCount = 0;

  @Input()
  roundOrder = 0;

  @Output()
  public submittedRound: EventEmitter<SeasonEventRoundDto> = new EventEmitter();

  roundTypeOptions: SelectItem[] = [
    {
      label: 'Ladder',
      value: RoundType.Ladder,
    },
    {
      label: 'Group',
      value: RoundType.Group,
    },
    {
      label: 'Classic Final',
      value: RoundType.ClassicFinal,
    },
    {
      label: 'Candidates Final',
      value: RoundType.CandidateFinal,
    },
  ];

  pointsStrategyOptions: SelectItem[] = [
    {
      label: 'Only this round',
      value: RoundPointsStrategy.OnlyThisRound,
    },
    {
      label: 'This and previous rounds',
      value: RoundPointsStrategy.SummedWithPreviousRound,
    },
  ];

  droppedPositionDefinementStrategyOptions: SelectItem[] = [
    {
      label: 'Position, then points',
      value: DroppedCarsPositionDefinementStrategy.RacePositionThenPoints,
    },
    {
      label: 'Only points',
      value: DroppedCarsPositionDefinementStrategy.OnlyPoints,
    },
  ];

  form: FormGroup = new FormGroup({});
  racesParticipantsCount: number[] = [];
  isSubmitButtonLoading = false;

  private racesCount = 0;
  private racesDtos: SeasonEventRoundRaceDto[] = [];
  private subscription = new Subscription();

  constructor(private fb: FormBuilder, private utils: UtilsService) {}

  ngOnInit(): void {
    this.refreshForm();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public refreshForm(): void {
    this.racesDtos = [];
    this.racesParticipantsCount = [];

    this.form = this.fb.group({
      type: [null as RoundType | null, [Validators.required]],
      pointsStrategy: [null as SelectItem | null, [Validators.required]],
      droppedCarsPositionDefinementStrategy: [
        null as SelectItem | null,
        [Validators.required],
      ],
      droppedCarsPointsStrategy: [
        null as SelectItem | null,
        [Validators.required],
      ],
      carsPerRaceHelper: [0, [Validators.required, Validators.min(1)]],
      instantAdvancementsHelper: [0, [Validators.required, Validators.min(1)]],
      secondChancesHelper: [0, [Validators.required, Validators.min(0)]],
    });

    const s = this.form
      .get('carsPerRaceHelper')
      ?.valueChanges.subscribe(carsPerRace =>
        this.handleCarsPerRaceChange(carsPerRace)
      );

    if (s != null) {
      this.subscription.add(s);
    }
  }

  onSubmit(): void {
    console.warn(this.form.getRawValue());

    const type = this.utils.getUndefinedableOrThrow(
      this.form.get('type')?.value as SelectItem | undefined
    ).value;

    const pointsStrategy = this.utils.getUndefinedableOrThrow(
      this.form.get('pointsStrategy')?.value as SelectItem | undefined
    ).value;

    const droppedCarsPositionDefinementStrategy =
      this.utils.getUndefinedableOrThrow(
        this.form.get('droppedCarsPositionDefinementStrategy')?.value as
          | SelectItem
          | undefined
      ).value;

    const droppedCarsPointsStrategy = this.utils.getUndefinedableOrThrow(
      this.form.get('droppedCarsPointsStrategy')?.value as
        | SelectItem
        | undefined
    ).value;

    const roundDto = new SeasonEventRoundDto(
      this.roundOrder,
      type,
      this.participantsCount,
      [],
      this.racesDtos,
      pointsStrategy,
      droppedCarsPositionDefinementStrategy,
      droppedCarsPointsStrategy,
      undefined,
      undefined
    );

    this.submittedRound.emit(roundDto);
  }

  handleRaceSubmitted(raceDto: SeasonEventRoundRaceDto): void {
    const raceOrder = raceDto.order;

    this.racesDtos = this.racesDtos.filter(r => r.order !== raceOrder);
    this.racesDtos.push(raceDto);
  }

  private handleCarsPerRaceChange(carsPerRace: number): void {
    if (carsPerRace <= 0) {
      console.error('Cars per race must be greater than 0');
      return;
    }

    this.racesParticipantsCount = [];
    this.racesDtos = [];

    this.racesCount = Math.floor(this.participantsCount / carsPerRace);

    for (let i = 0; i < this.racesCount; i++) {
      this.racesParticipantsCount.push(carsPerRace);
    }

    const reminder = this.participantsCount - this.racesCount * carsPerRace;

    if (reminder > 0) {
      this.racesCount += 1;
      this.racesParticipantsCount.push(reminder);
    }
  }
}
