import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { RouteTitleService } from 'src/app/services/route-title.service';

@Component({
  selector: 'app-title-bar',
  templateUrl: './title-bar.component.html',
  styleUrls: ['./title-bar.component.css'],
})
export class TitleBarComponent implements OnInit, OnDestroy {
  public routeTitle: string | undefined;

  private subscription = new Subscription();

  constructor(private routeTitleService: RouteTitleService) {}

  ngOnInit(): void {
    const s = this.routeTitleService
      .getRouteTitle()
      .subscribe(title => (this.routeTitle = title));
    this.subscription.add(s);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
