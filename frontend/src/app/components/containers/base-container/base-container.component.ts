import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';
import { PicoBoardsService } from 'src/app/services/pico-boards.service';
import {
  RouteTitle,
  RouteTitleService,
} from 'src/app/services/route-title.service';
import { WebSocketService } from 'src/app/services/websocket.service';

@Component({
  selector: 'app-base-container',
  templateUrl: './base-container.component.html',
  styleUrls: ['./base-container.component.css'],
})
export class BaseContainerComponent implements OnInit, OnDestroy {
  protected subscription = new Subscription();

  constructor(
    private routeTitleService: RouteTitleService,
    private route: ActivatedRoute,
    public backendService: BackendService,
    public webSocketService: WebSocketService,
    public picoBoardsService: PicoBoardsService
  ) {}

  ngOnInit(): void {
    const s = this.route.title.subscribe(title => {
      const routeTitle = title as RouteTitle;
      this.routeTitleService.setRouteTitle(routeTitle);
    });
    this.subscription.add(s);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
