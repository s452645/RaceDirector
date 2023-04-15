import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SelectItem, SelectItemGroup } from 'primeng/api';
import { Subscription } from 'rxjs';
import {
  CheckpointDto,
  CheckpointType,
  CircuitDto,
} from 'src/app/services/seasons/events/circuit.service';
import { PicoBoardsService } from 'src/app/services/hardware/pico-boards.service';
import { UtilsService } from 'src/app/services/utils.service';

interface CheckpointTypeForm {
  name: string;
  code: number;
}

@Component({
  selector: 'app-circuit-form',
  templateUrl: './circuit-form.component.html',
  styleUrls: ['./circuit-form.component.css'],
})
export class CircuitFormComponent implements OnInit, OnDestroy {
  @Input()
  public circuitDto: CircuitDto | null | undefined;

  @Output()
  public submittedForm: EventEmitter<CircuitDto> = new EventEmitter();

  groupedBreakBeamSensors: SelectItemGroup[] = [];

  checkpointTypes: CheckpointTypeForm[] = [
    { name: 'Continue', code: CheckpointType.Continue },
    { name: 'Start', code: CheckpointType.Start },
    { name: 'Stop', code: CheckpointType.Stop },
    { name: 'Pause', code: CheckpointType.Pause },
    { name: 'Resume', code: CheckpointType.Resume },
  ];

  form = this.fb.group({
    name: ['', Validators.required],
    checkpoints: this.fb.array([this.createCheckpointFormGroup()]),
  });

  get checkpoints() {
    return this.form.get('checkpoints') as FormArray;
  }

  public isSubmitButtonLoading = false;

  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private utils: UtilsService,
    private picoBoardsService: PicoBoardsService
  ) {}

  ngOnInit(): void {
    this.subscription.add(
      this.picoBoardsService
        .getBoards()
        .subscribe(boards => {
          this.groupedBreakBeamSensors = boards.map(board => {
            return {
              label: board.name,
              value: this.utils.getUndefinedableOrThrow(board.id),
              items: board.breakBeamSensors.map(sensor => {
                return {
                  label: `${sensor.name} (pin ${sensor.pin})`,
                  value: this.utils.getUndefinedableOrThrow(sensor.id),
                };
              }),
            };
          });
        })
        .add(() => this.parseDtoToForm())
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  refreshForm(): void {
    this.form.reset();

    this.form.controls.checkpoints = this.fb.array([
      this.createCheckpointFormGroup(),
    ]);

    this.parseDtoToForm();
  }

  addCheckpoint(): void {
    this.checkpoints.push(this.createCheckpointFormGroup());
    this.form.updateValueAndValidity();
  }

  removeCheckpoint(): void {
    this.checkpoints.removeAt(-1);
    this.form.updateValueAndValidity();
  }

  onSubmit(): void {
    this.isSubmitButtonLoading = true;

    const circuitName = this.utils.getNullableOrThrow(
      this.form.controls.name.value
    );
    const checkpointsArray = this.form.controls.checkpoints as FormArray;

    const checkpoints = checkpointsArray.controls.map((checkpoint, idx) => {
      const name = checkpoint.get('checkpointName')?.value;

      const typeForm = checkpoint.get('type')?.value as CheckpointTypeForm;
      const type = typeForm.code;

      const sensor = checkpoint.get('sensor')?.value as SelectItem;
      return new CheckpointDto(name, idx, type, sensor.value);
    });

    const circuit = new CircuitDto(circuitName, checkpoints);
    this.submittedForm.emit(circuit);
  }

  private createCheckpointFormGroup(
    name?: string,
    type?: CheckpointType,
    sensorId?: string
  ): FormGroup {
    const formGroup = this.fb.group({
      checkpointName: ['', Validators.required],
      type: [null as CheckpointTypeForm | null, Validators.required],
      sensor: [null as SelectItem | null, Validators.required],
    });

    if (name !== undefined) {
      formGroup.controls.checkpointName.setValue(name);
    }

    if (type !== undefined) {
      formGroup.controls.type.setValue(
        this.checkpointTypes.find(
          formType => formType.code === type.valueOf()
        ) ?? null
      );
    }

    if (sensorId !== undefined) {
      formGroup.controls.sensor.setValue(
        this.groupedBreakBeamSensors
          .map(group => group.items)
          .flat()
          .find(item => item.value === sensorId) ?? null
      );
    }

    return formGroup;
  }

  private parseDtoToForm(): void {
    if (this.circuitDto) {
      this.form.controls.name.setValue(this.circuitDto.name);

      this.form.controls.checkpoints.clear();
      this.circuitDto.checkpoints
        .map(checkpoint =>
          this.createCheckpointFormGroup(
            checkpoint.name,
            checkpoint.type,
            checkpoint.breakBeamSensorId
          )
        )
        .forEach(checkpoint => this.form.controls.checkpoints.push(checkpoint));

      this.form.updateValueAndValidity();
    }

    this.form.controls.checkpoints.valueChanges.subscribe(() =>
      this.form.updateValueAndValidity({ onlySelf: true })
    );
  }
}
