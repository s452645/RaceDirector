import { Injectable } from '@angular/core';
import { BackendService } from '../backend.service';
import { Observable } from 'rxjs';

const URL = 'https://localhost:7219/api/Elevator';

@Injectable({
  providedIn: 'root',
})
export class ElevatorService {
  constructor(private backendService: BackendService) {}

  public sendCommand(command: string): Observable<void> {
    return this.backendService.post(`${URL}/command?command=${command}`, null);
  }
}
