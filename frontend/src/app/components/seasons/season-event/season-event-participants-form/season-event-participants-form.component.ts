import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { CarDto, CarService } from 'src/app/services/cars/car.service';
import { SeasonsService } from 'src/app/services/seasons/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event-participants-form',
  templateUrl: './season-event-participants-form.component.html',
  styleUrls: ['./season-event-participants-form.component.css'],
})
export class SeasonEventParticipantsFormComponent implements OnInit, OnDestroy {
  @Input()
  public seasonEventId: string | null = null;

  @Output()
  public submittedPatricipantsIds: EventEmitter<string[]> = new EventEmitter();

  isSubmitButtonLoading = false;

  public cars: CarDto[] = [];
  public participants: string[] = [];

  private subsription = new Subscription();

  constructor(
    private utils: UtilsService,
    private carService: CarService,
    private seasonsService: SeasonsService
  ) {}

  ngOnInit(): void {
    this.subsription.add(
      this.carService.getAllCars().subscribe(cars => {
        this.cars = cars;

        this.subsription.add(
          this.seasonsService
            .getSeasonEventParticipants(
              this.utils.getNullableOrThrow(this.seasonEventId)
            )
            .subscribe(participants => {
              this.participants = participants.map(p => p.id || '');
            })
        );
      })
    );
  }

  ngOnDestroy(): void {
    this.subsription.unsubscribe();
  }

  onSubmit(): void {
    this.isSubmitButtonLoading = true;
    this.submittedPatricipantsIds.emit(this.participants);
  }
}
