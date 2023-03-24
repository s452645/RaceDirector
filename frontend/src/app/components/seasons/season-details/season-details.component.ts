import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import {
  SeasonEventDto,
  SeasonsService,
} from 'src/app/services/seasons.service';

@Component({
  selector: 'app-season-details',
  templateUrl: './season-details.component.html',
  styleUrls: ['./season-details.component.css'],
})
export class SeasonDetailsComponent implements OnInit, OnDestroy {
  public seasonId: string | null = null;
  public seasonEvents: SeasonEventDto[] = [];

  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private seasonService: SeasonsService
  ) {}

  ngOnInit(): void {
    this.seasonId = this.route.snapshot.paramMap.get('id');

    if (this.seasonId === null) {
      console.error('Could not initialized view. Season ID cannot be empty.');
      return;
    }

    this.subscription.add(
      this.seasonService
        .getSeasonEvents(this.seasonId)
        .subscribe(events => (this.seasonEvents = events))
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
