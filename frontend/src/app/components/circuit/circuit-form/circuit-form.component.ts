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
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SelectItem, SelectItemGroup } from 'primeng/api';
import { Subscription } from 'rxjs';
import {
  CheckpointDto,
  CheckpointType,
  CircuitDto,
  CircuitService,
  Track,
} from 'src/app/services/seasons/events/circuit.service';
import { PicoBoardsService } from 'src/app/services/hardware/pico-boards.service';
import { UtilsService } from 'src/app/services/utils.service';

const CIRCUIT_NAME_FIELD = 'name';
const CHECKPOINTS_FIELD = 'checkpoints';

const CHECKPOINT_NAME_FIELD = 'checkpointName';
const CHECKPOINT_TYPE_FIELD = 'type';
const CHECKPOINT_SENSOR_FIELD = 'sensor';
const CHECKPOINT_TRACK_FIELD = 'track';

@Component({
  selector: 'app-circuit-form',
  templateUrl: './circuit-form.component.html',
  styleUrls: ['./circuit-form.component.css'],
})
export class CircuitFormComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  public seasonEventId: string | undefined;

  @Input()
  public existingCircuit: CircuitDto | undefined;

  @Output()
  public closeSelf = new EventEmitter<boolean>();

  groupedBreakBeamSensors: SelectItemGroup[] = [];

  checkpointTypes: SelectItem[] = [
    { label: 'Continue', value: CheckpointType.Continue },
    { label: 'Start', value: CheckpointType.Start },
    { label: 'Stop', value: CheckpointType.Stop },
    { label: 'Pause', value: CheckpointType.Pause },
    { label: 'Resume', value: CheckpointType.Resume },
  ];

  tracks: SelectItem[] = [
    { label: 'All', value: Track.ALL },
    { label: 'Track A', value: Track.TRACK_A },
    { label: 'Track B', value: Track.TRACK_B },
    { label: 'Track C', value: Track.TRACK_C },
    { label: 'Track D', value: Track.TRACK_D },
  ];

  get checkpoints() {
    return this.form.get(CHECKPOINTS_FIELD) as FormArray;
  }

  public form: FormGroup = this.fb.group({});
  public isSubmitButtonLoading = false;

  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private utils: UtilsService,
    private picoBoardsService: PicoBoardsService,
    private circuitService: CircuitService
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
        .add(() => this.initForm())
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['existingCircuit']) {
      this.initForm();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initForm(): void {
    const circuitName = this.existingCircuit?.name ?? null;
    const checkpoints = this.existingCircuit?.checkpoints.map(c =>
      this.initCheckpointForm(c)
    ) ?? [this.initCheckpointForm()];

    this.form = this.fb.group({
      [CIRCUIT_NAME_FIELD]: [circuitName, Validators.required],
      [CHECKPOINTS_FIELD]: this.fb.array(checkpoints),
    });

    this.form.updateValueAndValidity();
  }

  initCheckpointForm(checkpoint?: CheckpointDto): FormGroup {
    const checkpointName = checkpoint?.name ?? null;
    const type =
      this.checkpointTypes.find(c => c.value === checkpoint?.type) ?? null;
    const sensor =
      this.groupedBreakBeamSensors
        .map(group => group.items)
        .flat()
        .find(item => item.value === checkpoint?.breakBeamSensorId) ?? null;
    const track = this.tracks.find(t => t.value === checkpoint?.track);

    return this.fb.group({
      [CHECKPOINT_NAME_FIELD]: [checkpointName, Validators.required],
      [CHECKPOINT_TYPE_FIELD]: [type, Validators.required],
      [CHECKPOINT_SENSOR_FIELD]: [sensor, Validators.required],
      [CHECKPOINT_TRACK_FIELD]: [track, [Validators.required]],
    });
  }

  addCheckpoint(): void {
    this.checkpoints.push(this.initCheckpointForm());
    this.form.updateValueAndValidity();
  }

  removeCheckpoint(): void {
    this.checkpoints.removeAt(-1);
    this.form.updateValueAndValidity();
  }

  onSubmit(): void {
    this.isSubmitButtonLoading = true;

    const circuitName = this.utils.getNullableOrThrow(
      this.form.controls[CIRCUIT_NAME_FIELD].value
    );

    const checkpoints = this.checkpoints.controls.map((checkpoint, idx) => {
      const checkpointForm = checkpoint as FormGroup;

      const name = checkpointForm.controls[CHECKPOINT_NAME_FIELD]?.value;

      const type = checkpointForm.controls[CHECKPOINT_TYPE_FIELD]
        ?.value as SelectItem;

      const sensor = checkpointForm.controls[CHECKPOINT_SENSOR_FIELD]
        ?.value as SelectItem;

      const track = checkpointForm.controls[CHECKPOINT_TRACK_FIELD]
        ?.value as SelectItem;

      return new CheckpointDto(
        name,
        idx,
        type.value,
        sensor.value,
        track.value
      );
    });

    const circuit = new CircuitDto(circuitName, checkpoints);

    if (this.existingCircuit) {
      circuit.id = this.existingCircuit.id;
      this.updateCircuit(circuit);
      return;
    }

    this.addCircuit(circuit);
  }

  private addCircuit(circuitDto: CircuitDto): void {
    this.subscription.add(
      this.circuitService
        .addCircuit(
          this.utils.getUndefinedableOrThrow(this.seasonEventId),
          circuitDto
        )
        .subscribe(() => {
          this.closeSelf.emit(true);
          this.initForm();
        })
        .add(() => (this.isSubmitButtonLoading = false))
    );
  }

  private updateCircuit(circuitDto: CircuitDto): void {
    this.subscription.add(
      this.circuitService
        .updateCircuit(
          this.utils.getUndefinedableOrThrow(this.seasonEventId),
          circuitDto
        )
        .subscribe(() => {
          this.closeSelf.emit(true);
          this.initForm();
        })
        .add(() => (this.isSubmitButtonLoading = false))
    );
  }
}
