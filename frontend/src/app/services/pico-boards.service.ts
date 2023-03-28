import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { MOCK_BACKEND } from '../globals';
import { BackendService } from './backend.service';

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
    public name: string,
    public ipAddress: string,
    public breakBeamSensors: BreakBeamSensorDto[]
  ) {}
}

const URL = 'https://localhost:7219/api/PicoBoards';

// =========== OLD ============

// TODO: remove?
export interface DEPPicoWBoardDto {
  id: string;
  address: string;
  isConnected: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class PicoBoardsService {
  constructor(private backendService: BackendService) {}

  public getBoards(): Observable<PicoBoardDto[]> {
    return this.backendService.get<PicoBoardDto[]>(URL);
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

  public DEPgetBoards(): Observable<DEPPicoWBoardDto[]> {
    if (MOCK_BACKEND) {
      return of(this.getMockBoards());
    }

    return this.backendService.get<DEPPicoWBoardDto[]>(
      'https://localhost:7219/api/Board'
    );
  }

  public DEPaddBoard(
    boardId: string,
    boardAddress: string
  ): Observable<string> {
    const requestBody = {
      id: boardId,
      address: boardAddress,
    };

    return this.backendService.post(
      'https://localhost:7219/api/Board/addBoard',
      requestBody
    );
  }

  private getMockBoards(): DEPPicoWBoardDto[] {
    return [
      {
        id: 'mock-board-0-id',
        address: 'mock-board-address',
        isConnected: true,
      },

      {
        id: 'mock-board-1-id',
        address: 'mock-board-address',
        isConnected: true,
      },

      {
        id: 'mock-board-2-id',
        address: 'mock-board-address',
        isConnected: true,
      },
    ];
  }
}
