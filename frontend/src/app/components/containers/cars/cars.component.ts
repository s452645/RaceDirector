import { Component } from '@angular/core';
import { BaseContainerComponent } from '../base-container/base-container.component';

@Component({
  selector: 'app-cars',
  templateUrl: './cars.component.html',
  styleUrls: ['./cars.component.css'],
})
export class CarsComponent extends BaseContainerComponent {}
