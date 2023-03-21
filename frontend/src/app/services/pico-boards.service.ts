import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { MOCK_BACKEND } from '../globals';
import { BackendService } from './backend.service';

export interface PicoWBoardDto {
  id: string;
  address: string;
  isConnected: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class PicoBoardsService {
  constructor(private backendService: BackendService) {}

  public getBoards(): Observable<PicoWBoardDto[]> {
    if (MOCK_BACKEND) {
      return of(this.getMockBoards());
    }

    return this.backendService.get<PicoWBoardDto[]>(
      'https://localhost:7219/api/Board'
    );
  }

  public addBoard(boardId: string, boardAddress: string): Observable<string> {
    const requestBody = {
      id: boardId,
      address: boardAddress,
    };

    return this.backendService.post(
      'https://localhost:7219/api/Board/addBoard',
      requestBody
    );
  }

  private getMockBoards(): PicoWBoardDto[] {
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
