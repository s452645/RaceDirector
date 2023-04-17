import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
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
  SeasonEventRoundsService,
} from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

const TYPE_FIELD = 'type';
const POINTS_STRATEGY_FIELD = 'pointsStrategy';
const DROPPED_CARS_POSITION_DEFINEMENT_FIELD =
  'droppedCarsPositionDefinementStrategy';
const DROPPED_CARS_POINTS_FIELD = 'droppedCarsPointsStrategy';
const CARS_PER_RACE_FIELD = 'carsPerRaceHelper';
const INSTANT_ADVANCEMENTS_FIELD = 'instantAdvancementsHelper';
const SECOND_CHANCE_FIELD = 'secondChancesHelper';

const roundTypeOptions: SelectItem[] = [
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

const pointsStrategyOptions: SelectItem[] = [
  {
    label: 'Only this round',
    value: RoundPointsStrategy.OnlyThisRound,
  },
  {
    label: 'This and previous rounds',
    value: RoundPointsStrategy.SummedWithPreviousRound,
  },
];

const droppedPositionDefinementStrategyOptions: SelectItem[] = [
  {
    label: 'Position, then points',
    value: DroppedCarsPositionDefinementStrategy.RacePositionThenPoints,
  },
  {
    label: 'Only points',
    value: DroppedCarsPositionDefinementStrategy.OnlyPoints,
  },
];

@Component({
  selector: 'app-round-form',
  templateUrl: './round-form.component.html',
  styleUrls: ['./round-form.component.css'],
})
export class RoundFormComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  participantsCount = 0;

  @Input()
  roundOrder = 0;

  @Input()
  seasonEventId: string | undefined;

  @Input()
  existingRound: SeasonEventRoundDto | undefined;

  @Output()
  public closeSelf: EventEmitter<boolean> = new EventEmitter();

  form: FormGroup = new FormGroup({});
  racesParticipantsCount: number[] = [];
  isSubmitButtonLoading = false;

  public roundTypeOptions: SelectItem[] = [];
  public pointsStrategyOptions: SelectItem[] = [];
  public droppedPositionDefinementStrategyOptions: SelectItem[] = [];

  private racesCount = 0;
  private racesDtos: SeasonEventRoundRaceDto[] = [];
  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private roundService: SeasonEventRoundsService,
    private utils: UtilsService
  ) {}

  ngOnInit(): void {
    this.roundTypeOptions = roundTypeOptions;
    this.pointsStrategyOptions = pointsStrategyOptions;
    this.droppedPositionDefinementStrategyOptions =
      droppedPositionDefinementStrategyOptions;

    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['existingRound']) {
      this.initForm();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onSubmit(): void {
    const roundDto = this.getDtoFromForm();

    if (this.existingRound) {
      roundDto.id = this.existingRound.id;
      this.updateRound(roundDto);
      return;
    }

    this.addRound(roundDto);
  }

  // TODO
  handleRaceSubmitted(raceDto: SeasonEventRoundRaceDto): void {
    const raceOrder = raceDto.order;

    this.racesDtos = this.racesDtos.filter(r => r.order !== raceOrder);
    this.racesDtos.push(raceDto);
  }

  private initForm(): void {
    this.racesDtos = this.existingRound?.races ?? [];
    this.racesParticipantsCount = [];

    const type =
      this.roundTypeOptions.find(
        opt => opt.value === this.existingRound?.type
      ) ?? null;

    const pointsStrategy =
      this.pointsStrategyOptions.find(
        opt => opt.value === this.existingRound?.pointsStrategy
      ) ?? null;

    const droppedCarsPositionDefinementStrategy =
      this.droppedPositionDefinementStrategyOptions.find(
        opt =>
          opt.value ===
          this.existingRound?.droppedCarsPositionDefinementStrategy
      ) ?? null;

    const droppedCarsPointsStrategy =
      this.pointsStrategyOptions.find(
        opt => opt.value === this.existingRound?.droppedCarsPointsStrategy
      ) ?? null;

    this.form = this.fb.group({
      [TYPE_FIELD]: [type, [Validators.required]],
      [POINTS_STRATEGY_FIELD]: [pointsStrategy, [Validators.required]],
      [DROPPED_CARS_POSITION_DEFINEMENT_FIELD]: [
        droppedCarsPositionDefinementStrategy,
        [Validators.required],
      ],
      [DROPPED_CARS_POINTS_FIELD]: [
        droppedCarsPointsStrategy,
        [Validators.required],
      ],
      [CARS_PER_RACE_FIELD]: [0],
      [INSTANT_ADVANCEMENTS_FIELD]: [0],
      [SECOND_CHANCE_FIELD]: [0],
    });

    const s = this.form.controls[CARS_PER_RACE_FIELD]?.valueChanges.subscribe(
      carsPerRace => this.handleCarsPerRaceChange(carsPerRace)
    );

    if (s != null) {
      this.subscription.add(s);
    }
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

  private getDtoFromForm(): SeasonEventRoundDto {
    const type = this.form.get(TYPE_FIELD)?.value.value;

    const pointsStrategy = this.form.get(POINTS_STRATEGY_FIELD)?.value.value;

    const droppedCarsPositionDefinementStrategy = this.form.get(
      DROPPED_CARS_POSITION_DEFINEMENT_FIELD
    )?.value.value;

    const droppedCarsPointsStrategy = this.form.get(DROPPED_CARS_POINTS_FIELD)
      ?.value.value;

    return new SeasonEventRoundDto(
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
  }

  private addRound(roundDto: SeasonEventRoundDto): void {
    this.isSubmitButtonLoading = true;

    const s = this.roundService
      .addRound(
        this.utils.getUndefinedableOrThrow(this.seasonEventId),
        roundDto
      )
      .subscribe(() => {
        this.closeSelf.emit(true);
        this.initForm();
      })
      .add(() => (this.isSubmitButtonLoading = false));

    this.subscription.add(s);
  }

  private updateRound(roundDto: SeasonEventRoundDto): void {
    this.isSubmitButtonLoading = true;

    const s = this.roundService
      .updateRound(
        this.utils.getUndefinedableOrThrow(this.seasonEventId),
        roundDto
      )
      .subscribe(() => {
        this.closeSelf.emit(true);
        this.initForm();
      })
      .add(() => (this.isSubmitButtonLoading = false));

    this.subscription.add(s);
  }
}
