import { Location } from '@angular/common';
import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  public history: string[] = [];
  public canGoBack: Subject<boolean> = new Subject();

  constructor(private router: Router, private location: Location) {
    this.canGoBack.next(false);

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.urlAfterRedirects !== this.history.at(-1))
          this.history.push(event.urlAfterRedirects);
      }

      this.canGoBack.next(this.history.length > 1);
    });
  }

  public back(): void {
    if (this.history.length > 0) {
      this.history.pop();
      this.location.back();
    }

    this.canGoBack.next(this.history.length > 1);
  }
}
