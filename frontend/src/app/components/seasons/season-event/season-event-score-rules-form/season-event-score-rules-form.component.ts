import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { SelectItem } from 'primeng/api';
import { SeasonEventScoreRulesDto } from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event-score-rules-form',
  templateUrl: './season-event-score-rules-form.component.html',
  styleUrls: ['./season-event-score-rules-form.component.css'],
})
export class SeasonEventScoreRulesFormComponent implements OnInit {
  @Input()
  public seasonEventId: string | null = null;

  @Input()
  public scoreRulesDto: SeasonEventScoreRulesDto | null | undefined = null;

  @Output()
  public submittedScoreRules: EventEmitter<SeasonEventScoreRulesDto> =
    new EventEmitter();

  theMoreTheBetterOptions: SelectItem[] = [
    {
      label: 'More',
      value: true,
    },
    {
      label: 'Less',
      value: false,
    },
  ];

  form: FormGroup = new FormGroup({});
  isSubmitButtonLoading = false;

  constructor(private fb: FormBuilder, private utils: UtilsService) {}

  ngOnInit(): void {
    this.refreshForm();
  }

  refreshForm(): void {
    this.formReset();
    this.parseDtoToForm();
  }

  onSubmit(): void {
    this.isSubmitButtonLoading = true;

    const timeMultiplier = this.utils.getNullableOrThrow(
      this.form.controls['timeMultiplier'].value
    );

    const distanceMultiplier = this.utils.getNullableOrThrow(
      this.form.controls['distanceMultiplier'].value
    );

    const availableBonuses =
      (this.form.controls['availableBonuses'].value as string[] | null)?.map(
        bonus => parseFloat(bonus)
      ) || [];

    const unfinishedSectorPenaltyPoints = this.utils.getNullableOrThrow(
      this.form.controls['unfinishedSectorPenaltyPoints'].value
    );

    const theMoreTheBetter = this.utils.getNullableOrThrow(
      this.form.controls['theMoreTheBetter'].value
    );

    this.submittedScoreRules.emit(
      new SeasonEventScoreRulesDto(
        timeMultiplier,
        distanceMultiplier,
        availableBonuses,
        unfinishedSectorPenaltyPoints,
        theMoreTheBetter,
        this.utils.getNullableOrThrow(this.seasonEventId)
      )
    );
  }

  private availableBonusesValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const stringValues = control.value as string[];

      if (!stringValues) {
        return null;
      }

      return stringValues.some(value => isNaN(parseFloat(value)))
        ? {
            invalidValues: {
              value: stringValues
                .filter(value => isNaN(parseFloat(value)))
                .join(', '),
            },
          }
        : null;
    };
  }

  private parseDtoToForm(): void {
    if (this.scoreRulesDto) {
      this.form.patchValue({
        timeMultiplier: this.scoreRulesDto.timeMultiplier,
        distanceMultiplier: this.scoreRulesDto.distanceMultiplier,
        availableBonuses: this.scoreRulesDto.availableBonuses.map(b =>
          b.toString()
        ),
        unfinishedSectorPenaltyPoints:
          this.scoreRulesDto.unfinishedSectorPenaltyPoints,
        theMoreTheBetter: this.scoreRulesDto.theMoreTheBetter,
      });

      this.form.updateValueAndValidity();
    }
  }

  private formReset(): void {
    this.form = this.fb.group({
      timeMultiplier: [0, [Validators.required, Validators.min(0.01)]],
      distanceMultiplier: [0, [Validators.required, Validators.min(0.01)]],
      availableBonuses: [[] as string[], [this.availableBonusesValidator()]],
      unfinishedSectorPenaltyPoints: [
        0,
        [Validators.required, Validators.min(0.01)],
      ],
      theMoreTheBetter: [true, Validators.required],
    });
  }
}
