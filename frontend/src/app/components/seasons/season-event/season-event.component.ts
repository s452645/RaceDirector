import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { CircuitDto, CircuitService } from 'src/app/services/circuit.service';
import { RouteTitleService } from 'src/app/services/route-title.service';
import {
  SeasonEventDto,
  SeasonsService,
} from 'src/app/services/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';
import { CircuitFormComponent } from '../../circuit/circuit-form/circuit-form.component';

@Component({
  selector: 'app-season-event',
  templateUrl: './season-event.component.html',
  styleUrls: ['./season-event.component.css'],
})
export class SeasonEventComponent implements OnInit, OnDestroy {
  @ViewChild(CircuitFormComponent, { static: true })
  private circuitFormCmp!: CircuitFormComponent;

  private seasonIdNullable: string | null = null;
  private seasonEventIdNullable: string | null = null;
  private seasonEventNullable: SeasonEventDto | null = null;
  private subscription = new Subscription();

  get seasonId(): string {
    return this.utils.getNullableOrThrow(this.seasonIdNullable);
  }

  get seasonEventId(): string {
    return this.utils.getNullableOrThrow(this.seasonEventIdNullable);
  }

  get seasonEvent(): SeasonEventDto {
    return this.utils.getNullableOrThrow(this.seasonEventNullable);
  }

  isCircuitFormOpen = false;

  constructor(
    private route: ActivatedRoute,
    private routeTitleService: RouteTitleService,
    private seasonsService: SeasonsService,
    private utils: UtilsService,
    private circuitService: CircuitService
  ) {
    this.seasonIdNullable = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventIdNullable = this.route.snapshot.paramMap.get('eventId');
  }

  ngOnInit(): void {
    this.subscription.add(
      this.seasonsService
        .getSeasonEventById(this.seasonId, this.seasonEventId)
        .subscribe(seasonEvent => {
          this.routeTitleService.setRouteTitle(seasonEvent.name);
          this.seasonEventNullable = seasonEvent;
        })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  handleOpenCircuitForm(): void {
    this.isCircuitFormOpen = true;
  }

  handleSubmittedCircuit(circuit: CircuitDto): void {
    this.subscription.add(
      this.circuitService
        .addCircuit(this.seasonEventId, circuit)
        .subscribe(() => this.refreshData())
        .add(() => {
          this.isCircuitFormOpen = false;
          this.circuitFormCmp.refreshForm();
        })
    );
  }

  private refreshData(): void {
    this.subscription.add(
      this.seasonsService
        .getSeasonEventById(this.seasonId, this.seasonEventId)
        .subscribe(seasonEvent => (this.seasonEventNullable = seasonEvent))
    );
  }
}
