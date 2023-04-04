import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';
import { PicoBoardsService } from 'src/app/services/hardware/pico-boards.service';
import {
  RouteTitle,
  RouteTitleService,
} from 'src/app/services/route-title.service';
import { SeasonsService } from 'src/app/services/seasons/seasons.service';
import { ToastMessageService } from 'src/app/services/toast-message.service';
import { UtilsService } from 'src/app/services/utils.service';
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
    protected route: ActivatedRoute,
    protected router: Router,
    protected seasonsService: SeasonsService,
    protected toastMessageService: ToastMessageService,
    public backendService: BackendService,
    public webSocketService: WebSocketService,
    public picoBoardsService: PicoBoardsService,
    protected fb: FormBuilder,
    protected utils: UtilsService
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
