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
  private $routeTitleSubject: BehaviorSubject<string> = new BehaviorSubject(
    RouteTitle.APP as string
  );

  public getRouteTitle(): Observable<string> {
    return this.$routeTitleSubject.asObservable();
  }

  public setRouteTitle(title: string): void {
    this.$routeTitleSubject.next(title);
  }
}
