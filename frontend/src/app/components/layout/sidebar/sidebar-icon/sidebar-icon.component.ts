import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-sidebar-icon',
  templateUrl: './sidebar-icon.component.html',
  styleUrls: ['./sidebar-icon.component.css'],
})
export class SidebarIconComponent {
  @Input()
  public isExpanded = false;

  @Input()
  public icon = '';

  @Input()
  public expandedText = '';
}
