import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BackendService } from '../backend.service';

export enum PicoBoardType {
  USB,
  WiFi,
}

export class BreakBeamSensorDto {
  public id: string | undefined;

  constructor(
    public name: string,
    public pin: number,
    public boardId: string
  ) {}
}

export class PicoBoardDto {
  public id: string | undefined;

  constructor(
    public type: PicoBoardType,
    public name: string,
    public ipAddress: string,
    public breakBeamSensors: BreakBeamSensorDto[]
  ) {}

  public static fromPayload(payload: PicoBoardDto): PicoBoardDto {
    const board = new PicoBoardDto(
      payload.type,
      payload.name,
      payload.ipAddress,
      payload.breakBeamSensors
    );

    board.id = payload.id;
    return board;
  }

  public get typeText(): string {
    if (this.type === PicoBoardType.USB) return 'USB';
    else if (this.type === PicoBoardType.WiFi) return 'WiFi';

    return 'Unknown';
  }
}

const URL = 'https://localhost:7219/api/PicoBoards';

@Injectable({
  providedIn: 'root',
})
export class PicoBoardsService {
  constructor(private backendService: BackendService) {}

  public getBoards(): Observable<PicoBoardDto[]> {
    return this.backendService
      .get<PicoBoardDto[]>(URL)
      .pipe(map(boards => boards.map(PicoBoardDto.fromPayload)));
  }

  public addBoard(picoBoardDto: PicoBoardDto): Observable<PicoBoardDto> {
    return this.backendService.post<PicoBoardDto, PicoBoardDto>(
      URL,
      picoBoardDto
    );
  }

  public deleteBoard(picoBoardId: string): Observable<void> {
    return this.backendService.delete(`${URL}/${picoBoardId}`);
  }

  public getSensors(picoBoardId: string): Observable<BreakBeamSensorDto[]> {
    return this.backendService.get(`${URL}/${picoBoardId}/sensors`);
  }

  public addSensor(
    picoBoardId: string,
    sensor: BreakBeamSensorDto
  ): Observable<PicoBoardDto> {
    return this.backendService.post<BreakBeamSensorDto, PicoBoardDto>(
      `${URL}/${picoBoardId}/sensors`,
      sensor
    );
  }

  public removeSensor(
    picoBoardId: string,
    sensorId: string
  ): Observable<PicoBoardDto> {
    return this.backendService.delete(
      `${URL}/${picoBoardId}/sensors/${sensorId}`
    );
  }
}
