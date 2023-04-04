import {
  Component,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import {
  BreakBeamSensorDto,
  PicoBoardsService,
} from 'src/app/services/hardware/pico-boards.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-board-sensors-details',
  templateUrl: './board-sensors-details.component.html',
  styleUrls: ['./board-sensors-details.component.css'],
})
export class BoardSensorsDetailsComponent
  implements OnInit, OnDestroy, OnChanges
{
  @Input()
  boardId = '';

  form: FormGroup = new FormGroup({});
  sensors: BreakBeamSensorDto[] = [];

  private subscription = new Subscription();

  constructor(
    private boardsService: PicoBoardsService,
    private fb: FormBuilder,
    private utils: UtilsService
  ) {}

  ngOnInit(): void {
    this.refreshData();
    this.refreshForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['boardId']?.currentValue !== changes['boardId']?.previousValue
    ) {
      this.refreshData();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  handleSubmit(): void {
    const name = this.utils.getUndefinedableOrThrow(
      this.form.get('name')?.value
    );

    const pin = this.utils.getUndefinedableOrThrow(this.form.get('pin')?.value);

    const sensor = new BreakBeamSensorDto(name, pin, this.boardId);
    this.subscription.add(
      this.boardsService.addSensor(this.boardId, sensor).subscribe(() => {
        this.refreshData();
        this.refreshForm();
      })
    );
  }

  handleDeleteSensor(sensorId: string): void {
    this.subscription.add(
      this.boardsService
        .removeSensor(this.boardId, sensorId)
        .subscribe(() => this.refreshData())
    );
  }

  public refreshData(): void {
    if (!this.boardId) {
      return;
    }

    this.subscription.add(
      this.boardsService
        .getSensors(this.boardId)
        .subscribe(sensors => (this.sensors = sensors))
    );
  }

  private refreshForm(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      pin: [0, Validators.required],
    });
  }
}
