import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BackendService } from '../../backend.service';

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
    public breakBeamSensorId: string,
    public trackNumber: number
  ) {}

  public static fromPayload(payload: any): CheckpointDto {
    const checkpoint = new CheckpointDto(
      payload?.name,
      payload?.position,
      payload?.type,
      payload?.breakBeamSensorId,
      payload?.trackNumber
    );

    checkpoint.id = payload?.id;
    return checkpoint;
  }
}

export class CircuitDto {
  public id: string | undefined;

  constructor(public name: string, public checkpoints: CheckpointDto[]) {}

  public static fromPayload(payload: any): CircuitDto {
    const circuit = new CircuitDto(
      payload?.name,
      payload?.checkpoints.map((c: any) => CheckpointDto.fromPayload(c))
    );

    circuit.id = payload?.id;
    return circuit;
  }
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
    return this.backendService
      .get<CircuitDto>(
        `${URL}?seasonEventId=${seasonEventId}&circuitId=${circuitId}`
      )
      .pipe(map(circuit => CircuitDto.fromPayload(circuit)));
  }

  public addCircuit(
    seasonEventId: string,
    circuit: CircuitDto
  ): Observable<CircuitDto> {
    return this.backendService
      .post<CircuitDto, CircuitDto>(
        `${URL}?seasonEventId=${seasonEventId}`,
        circuit
      )
      .pipe(map(circuit => CircuitDto.fromPayload(circuit)));
  }

  public updateCircuit(
    seasonEventId: string,
    circuitDto: CircuitDto
  ): Observable<CircuitDto> {
    return this.backendService
      .put(`${URL}?seasonEventId=${seasonEventId}`, circuitDto)
      .pipe(map(circuit => CircuitDto.fromPayload(circuit)));
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
