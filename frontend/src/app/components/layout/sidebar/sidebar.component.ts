import { Component } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent {
  isExpanded = false;
  icon = 'pi-angle-double-right';

  onToggleExpand() {
    this.isExpanded = !this.isExpanded;
    this.icon = this.isExpanded
      ? 'pi-angle-double-left'
      : 'pi-angle-double-right';
  }
}
