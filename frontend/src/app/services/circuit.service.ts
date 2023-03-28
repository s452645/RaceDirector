import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from './backend.service';

export enum CheckpointType {
  Start,
  Stop,
  Pause,
  Resume,
  Continue,
}

export class CheckpointDto {
  public id: string | undefined;

  constructor(
    public name: string,
    public position: number,
    public type: CheckpointType,
    public breakBeamSensorId: string
  ) {}
}

export class CircuitDto {
  public id: string | undefined;

  constructor(public name: string, public checkpoints: CheckpointDto[]) {}
}

const URL = `https://localhost:7219/api/Circuits`;

@Injectable({
  providedIn: 'root',
})
export class CircuitService {
  constructor(private backendService: BackendService) {}

  public getCircuitById(
    seasonEventId: string,
    circuitId: string
  ): Observable<CircuitDto> {
    return this.backendService.get<CircuitDto>(
      `${URL}?seasonEventId=${seasonEventId}&circuitId=${circuitId}`
    );
  }

  public addCircuit(
    seasonEventId: string,
    circuit: CircuitDto
  ): Observable<CircuitDto> {
    return this.backendService.post<CircuitDto, CircuitDto>(
      `${URL}?seasonEventId=${seasonEventId}`,
      circuit
    );
  }

  public deleteCircuit(
    seasonEventId: string,
    circuitId: string
  ): Observable<void> {
    return this.backendService.delete<void>(
      `${URL}/${circuitId}?seasonEventId=${seasonEventId}?`
    );
  }
}
