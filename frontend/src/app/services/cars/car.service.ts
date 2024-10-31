import { Injectable } from '@angular/core';
import { BackendService } from '../backend.service';
import { Observable, map } from 'rxjs';

export class CarDto {
  public id: string | undefined;

  constructor(public name: string, public note: string) {}

  public static fromPayload(payload: any): CarDto {
    const carDto = new CarDto(payload?.name, payload?.note);
    carDto.id = payload?.id;

    return carDto;
  }

  public static fromWebsocketPayload(payload: any): CarDto {
    const carDto = new CarDto(payload?.Name, payload?.Note);
    carDto.id = payload?.Id;

    return carDto;
  }
}

const URL = `https://localhost:7219/api/Cars`;

@Injectable({
  providedIn: 'root',
})
export class CarService {
  constructor(private backendService: BackendService) {}

  public getAllCars(): Observable<CarDto[]> {
    return this.backendService
      .get<CarDto[]>(URL)
      .pipe(map(cars => cars.map(car => CarDto.fromPayload(car))));
  }
}
