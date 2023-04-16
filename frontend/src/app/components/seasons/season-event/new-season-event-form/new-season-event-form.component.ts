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
  SeasonEventDto,
  SeasonEventType,
  SeasonsService,
} from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

const NAME_FIELD = 'name';
const TYPE_FIELD = 'type';
const START_DATE_FIELD = 'startDate';
const END_DATE_FIELD = 'endDate';

@Component({
  selector: 'app-new-season-event-form',
  templateUrl: './new-season-event-form.component.html',
  styleUrls: ['./new-season-event-form.component.css'],
})
export class NewSeasonEventFormComponent
  implements OnInit, OnDestroy, OnChanges
{
  @Input() seasonId: string | undefined;
  @Input() existingSeasonEvent: SeasonEventDto | undefined;
  @Output() closeSelf: EventEmitter<boolean> = new EventEmitter();

  protected isSubmitButtonLoading = false;
  protected seasonEventTypes: SelectItem[] = [
    {
      label: 'Race',
      value: SeasonEventType.Race,
    },
    {
      label: 'Time Trial',
      value: SeasonEventType.TimeTrial,
    },
  ];

  protected form: FormGroup = new FormGroup({});

  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private utils: UtilsService,
    private seasonsService: SeasonsService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['existingSeasonEvent']) {
      this.initForm();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  protected onSubmit(): void {
    if (!this.seasonId) {
      // TODO: throw error
      return;
    }

    this.isSubmitButtonLoading = true;
    const newEventDto = this.getDtoFromForm();

    if (this.existingSeasonEvent) {
      newEventDto.id = this.existingSeasonEvent.id;
      this.updateSeasonEvent(newEventDto);
      return;
    }

    this.addNewSeasonEvent(newEventDto);
  }

  private initForm(): void {
    const name = this.existingSeasonEvent?.name ?? null;

    const type =
      this.seasonEventTypes.find(
        t => t.value === this.existingSeasonEvent?.type
      ) ?? null;

    const startDate = this.existingSeasonEvent?.startDate ?? null;
    const endDate = this.existingSeasonEvent?.endDate ?? null;

    this.form = this.fb.group({
      name: [name, Validators.required],
      type: [type as SelectItem | null, Validators.required],
      startDate: [startDate],
      endDate: [endDate],
    });
  }

  private getDtoFromForm(): SeasonEventDto {
    const name = this.utils.getNullableOrThrow(
      this.form.controls[NAME_FIELD].value
    );

    const startDate = new Date(
      this.form.controls[START_DATE_FIELD].value ?? ''
    );
    const endDate = new Date(this.form.controls[END_DATE_FIELD].value ?? '');

    const typeItem = this.form.controls[TYPE_FIELD].value as SelectItem;
    const type = typeItem.value;

    return new SeasonEventDto(
      name,
      startDate,
      endDate,
      type,
      undefined,
      undefined,
      undefined
    );
  }

  private addNewSeasonEvent(newSeasonEvent: SeasonEventDto): void {
    this.subscription.add(
      this.seasonsService
        .addSeasonEvent(
          this.utils.getUndefinedableOrThrow(this.seasonId),
          newSeasonEvent
        )
        .subscribe(() => {
          this.closeSelf.emit(true);
          this.isSubmitButtonLoading = false;
          this.initForm();
        })
    );
  }

  private updateSeasonEvent(updatedSeasonEvent: SeasonEventDto): void {
    this.subscription.add(
      this.seasonsService
        .updateSeasonEvent(
          this.utils.getUndefinedableOrThrow(this.seasonId),
          updatedSeasonEvent
        )
        .subscribe(() => {
          this.closeSelf.emit(true);
          this.isSubmitButtonLoading = false;
          this.initForm();
        })
    );
  }
}
