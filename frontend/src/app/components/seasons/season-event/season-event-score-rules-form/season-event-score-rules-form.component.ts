import { Component, EventEmitter, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { SelectItem } from 'primeng/api';
import { SeasonEventScoreRulesDto } from 'src/app/services/seasons.service';

@Component({
  selector: 'app-season-event-score-rules-form',
  templateUrl: './season-event-score-rules-form.component.html',
  styleUrls: ['./season-event-score-rules-form.component.css'],
})
export class SeasonEventScoreRulesFormComponent {
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

  form = this.fb.group({
    timeMultiplier: [0, [Validators.required, Validators.min(0.01)]],
    distanceMultiplier: [0, [Validators.required, Validators.min(0.01)]],
    availableBonuses: [[] as number[], [this.availableBonusesValidator()]],
    unfinishedSectorPenaltyPoints: [
      0,
      [Validators.required, Validators.min(0.01)],
    ],
    theMoreTheBetter: [true, Validators.required],
  });

  isSubmitButtonLoading = false;

  constructor(private fb: FormBuilder) {}

  onSubmit(): void {
    console.warn(this.form.value);
  }

  private availableBonusesValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const stringValues = control.value as string[];

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
}
