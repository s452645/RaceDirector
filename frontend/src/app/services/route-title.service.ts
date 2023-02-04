import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export enum RouteTitle {
  APP = 'Race Director',
  HOME = 'Home',
  SEASONS = 'Seasons',
  CARS = 'Cars',
  OWNERS = 'Owners',
  SETTINGS = 'Settings',
}

@Injectable({
  providedIn: 'root',
})
export class RouteTitleService {
  private $routeTitleSubject = new BehaviorSubject(RouteTitle.APP);

  public getRouteTitle(): Observable<RouteTitle> {
    return this.$routeTitleSubject.asObservable();
  }

  public setRouteTitle(title: RouteTitle): void {
    this.$routeTitleSubject.next(title);
  }
}
