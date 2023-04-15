import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BackendService } from '../backend.service';
import { SyncBoardResult } from './sync-data.service';

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
    public active: boolean,
    public connected: boolean,
    public breakBeamSensors: BreakBeamSensorDto[],

    public lastSyncDateTime: Date | undefined,
    public lastSyncOffset: number | undefined,
    public lastSyncResult: SyncBoardResult | undefined
  ) {}

  public static fromPayload(payload: PicoBoardDto): PicoBoardDto {
    const board = new PicoBoardDto(
      payload.type,
      payload.name,
      payload.ipAddress,
      payload.active,
      payload.connected,
      payload.breakBeamSensors,
      new Date(`${payload.lastSyncDateTime}`),
      payload.lastSyncOffset,
      payload.lastSyncResult
    );

    board.id = payload.id;
    return board;
  }

  public get typeText(): string {
    if (this.type === PicoBoardType.USB) return 'USB';
    else if (this.type === PicoBoardType.WiFi) return 'WiFi';

    return 'Unknown';
  }

  public get lastSyncLocaleDate(): string {
    if (this.lastSyncDateTime) {
      return this.lastSyncDateTime.toLocaleTimeString();
    }

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

  public connectBoard(picoBoardId: string): Observable<PicoBoardDto> {
    return this.backendService.post(`${URL}/${picoBoardId}/connect`, null);
  }

  public syncBoardOnce(picoBoardId: string): Observable<void> {
    return this.backendService.post(`${URL}/${picoBoardId}/sync-once`, null);
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
