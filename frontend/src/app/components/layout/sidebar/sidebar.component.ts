import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { RouteTitleService } from 'src/app/services/route-title.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit, OnDestroy {
  public isExpanded = false;
  public icon = 'pi-angle-double-right';
  public selectedOption = '';

  private subscription = new Subscription();

  constructor(private routeTitleService: RouteTitleService) {}

  public ngOnInit(): void {
    const s = this.routeTitleService
      .getRouteTitle()

      // FIXME (look for a string in active route instead)
      .subscribe(title => (this.selectedOption = title));
    this.subscription.add(s);
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public onToggleExpand(): void {
    this.isExpanded = !this.isExpanded;
    this.icon = this.isExpanded
      ? 'pi-angle-double-left'
      : 'pi-angle-double-right';
  }

  public isSelected(option: string): boolean {
    return option === this.selectedOption;
  }
}
