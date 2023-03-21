import { Component } from '@angular/core';
import { BaseContainerComponent } from '../base-container/base-container.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent extends BaseContainerComponent {
  public makeTestRequest(message: string): void {
    this.backendService
      .textRequest(message)
      .subscribe(response => console.log(response));
  }
}
