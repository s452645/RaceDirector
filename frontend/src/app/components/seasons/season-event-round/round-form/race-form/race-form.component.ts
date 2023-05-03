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
import { Subscription } from 'rxjs';
import { SeasonEventRoundRaceDto } from 'src/app/services/seasons/events/rounds/season-event-rounds.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-race-form',
  templateUrl: './race-form.component.html',
  styleUrls: ['./race-form.component.css'],
})
export class RaceFormComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  suggestedParticipantsCount = 0;

  @Input()
  suggestedInstantAdvancements = 0;

  @Input()
  suggestedSecondChances = 0;

  @Input()
  raceOrder = 0;

  @Output()
  public submittedRace: EventEmitter<SeasonEventRoundRaceDto> =
    new EventEmitter();

  form: FormGroup = new FormGroup({});

  private subscription = new Subscription();

  constructor(private fb: FormBuilder, private utils: UtilsService) {}

  ngOnInit(): void {
    this.refreshForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['suggestedInstantAdvancements'] ||
      changes['suggestedSecondChances']
    ) {
      this.refreshForm();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onSubmit(): void {
    console.warn(this.form.getRawValue());
  }

  private refreshForm(): void {
    this.form = this.fb.group({
      participantsCount: [
        this.suggestedParticipantsCount,
        [Validators.required],
      ],
      instantAdvancements: [
        this.suggestedInstantAdvancements,
        [Validators.required],
      ],
      secondChances: [this.suggestedSecondChances, [Validators.required]],
    });

    this.emitRaceDto();
    this.subscription.add(
      this.form.valueChanges.subscribe(() => this.emitRaceDto())
    );
  }

  private emitRaceDto(): void {
    const participantsCount = this.utils.getUndefinedableOrThrow(
      this.form.get('participantsCount')?.value
    );

    const instantAdvancements = this.utils.getUndefinedableOrThrow(
      this.form.get('instantAdvancements')?.value
    );

    const secondChances = this.utils.getUndefinedableOrThrow(
      this.form.get('secondChances')?.value
    );

    const raceDto = new SeasonEventRoundRaceDto(
      this.raceOrder,
      participantsCount,
      [],
      [],
      instantAdvancements,
      secondChances
    );

    this.submittedRace.emit(raceDto);
  }
}
