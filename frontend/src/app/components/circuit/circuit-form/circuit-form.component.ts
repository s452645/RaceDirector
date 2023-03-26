import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import {
  CheckpointDto,
  CheckpointType,
  CircuitDto,
} from 'src/app/services/circuit.service';
import { PicoBoardsService } from 'src/app/services/pico-boards.service';
import { UtilsService } from 'src/app/services/utils.service';

interface CheckpointTypeForm {
  name: string;
  code: number;
}

interface BreakBeamSensorForm {
  name: string;
  sensorId: string;
}

@Component({
  selector: 'app-circuit-form',
  templateUrl: './circuit-form.component.html',
  styleUrls: ['./circuit-form.component.css'],
})
export class CircuitFormComponent implements OnInit, OnDestroy {
  @Output()
  public submittedForm: EventEmitter<CircuitDto> = new EventEmitter();

  breakBeamSensors: BreakBeamSensorForm[] = [];

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

  isSubmitButtonLoading = false;

  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private utils: UtilsService,
    private picoBoardsService: PicoBoardsService
  ) {}

  // this shit will backfire
  ngOnInit(): void {
    this.subscription.add(
      this.picoBoardsService.getBoards().subscribe(
        boards =>
          (this.breakBeamSensors = boards
            .map(board => {
              const boardName = board.name;
              return board.breakBeamSensors.map(bbs => {
                return {
                  name: `${boardName} - ${bbs.name} (pin ${bbs.pin})`,
                  sensorId: this.utils.getUndefinedableOrThrow(bbs.id),
                };
              });
            })
            .flat())
      )
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
    this.isSubmitButtonLoading = false;
  }

  addCheckpoint(): void {
    this.checkpoints.push(this.createCheckpointFormGroup());
  }

  removeCheckpoint(): void {
    this.checkpoints.removeAt(-1);
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

      const sensorForm = checkpoint.get('sensorId')
        ?.value as BreakBeamSensorForm;
      const sensorId = sensorForm.sensorId;

      return new CheckpointDto(name, idx, type, sensorId);
    });

    const circuit = new CircuitDto(circuitName, checkpoints);

    this.submittedForm.emit(circuit);
  }

  private createCheckpointFormGroup(): FormGroup {
    return this.fb.group({
      checkpointName: ['', Validators.required],
      type: ['', Validators.required],
      sensorId: ['', Validators.required],
    });
  }
}
