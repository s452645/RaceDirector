import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { RouteTitleService } from 'src/app/services/route-title.service';
import {
  SeasonEventDto,
  SeasonsService,
} from 'src/app/services/seasons.service';
import { UtilsService } from 'src/app/services/utils.service';

@Component({
  selector: 'app-season-event',
  templateUrl: './season-event.component.html',
  styleUrls: ['./season-event.component.css'],
})
export class SeasonEventComponent implements OnInit, OnDestroy {
  private seasonId: string | null = null;
  private seasonEventId: string | null = null;
  private seasonEvent: SeasonEventDto | null = null;
  private subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private routeTitleService: RouteTitleService,
    private seasonsService: SeasonsService,
    private utils: UtilsService
  ) {
    this.seasonId = this.route.snapshot.paramMap.get('seasonId');
    this.seasonEventId = this.route.snapshot.paramMap.get('eventId');
  }

  ngOnInit(): void {
    this.subscription.add(
      this.seasonsService
        .getSeasonEventById(
          this.utils.getNullableOrThrow(this.seasonId),
          this.utils.getNullableOrThrow(this.seasonEventId)
        )
        .subscribe(seasonEvent => {
          this.routeTitleService.setRouteTitle(seasonEvent.name);
          this.seasonEvent = seasonEvent;
          console.log(this.seasonEvent);
        })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
