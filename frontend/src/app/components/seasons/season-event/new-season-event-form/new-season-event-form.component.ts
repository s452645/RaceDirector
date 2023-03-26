import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { SeasonEventDto } from 'src/app/services/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-new-season-event-form',
  templateUrl: './new-season-event-form.component.html',
  styleUrls: ['./new-season-event-form.component.css'],
})
export class NewSeasonEventFormComponent {
  @Output() newSeasonEvent: EventEmitter<SeasonEventDto> = new EventEmitter();

  protected isSubmitButtonLoading = false;
  protected form = this.fb.group({
    name: ['', Validators.required],
    startDate: [''],
    endDate: [''],
  });

  constructor(private fb: FormBuilder, private utils: UtilsService) {}

  public refreshForm(): void {
    this.isSubmitButtonLoading = false;
    this.form.reset();
  }

  protected onSubmit(): void {
    const name = this.utils.getNullableOrThrow(this.form.controls.name.value);
    const startDate = new Date(this.form.controls.startDate.value ?? '');
    const endDate = new Date(this.form.controls.endDate.value ?? '');

    this.isSubmitButtonLoading = true;
    this.newSeasonEvent.emit(
      new SeasonEventDto(name, startDate, endDate, undefined)
    );
  }
}
