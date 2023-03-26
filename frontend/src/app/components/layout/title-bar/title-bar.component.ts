import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { NavigationService } from 'src/app/services/navigation.service';
import { RouteTitleService } from 'src/app/services/route-title.service';

@Component({
  selector: 'app-title-bar',
  templateUrl: './title-bar.component.html',
  styleUrls: ['./title-bar.component.css'],
})
export class TitleBarComponent implements OnInit, OnDestroy {
  public routeTitle: string | undefined;
  public canGoBack = false;

  private subscription = new Subscription();

  constructor(
    private routeTitleService: RouteTitleService,
    private navigationService: NavigationService
  ) {}

  ngOnInit(): void {
    const s1 = this.navigationService.canGoBack.subscribe(
      canGoBack => (this.canGoBack = canGoBack)
    );
    this.subscription.add(s1);

    const s2 = this.routeTitleService
      .getRouteTitle()
      .subscribe(title => (this.routeTitle = title));
    this.subscription.add(s2);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public goBack(): void {
    if (this.canGoBack) {
      this.navigationService.back();
    }
  }
}
