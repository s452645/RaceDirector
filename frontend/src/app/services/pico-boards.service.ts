import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from './backend.service';

export class PicoWBoardDto {
  constructor(
    public id: string,
    public address: string,
    public isConnected: boolean
  ) {}
}

@Injectable({
  providedIn: 'root',
})
export class PicoBoardsService {
  constructor(private backendService: BackendService) {}

  public getBoards(): Observable<PicoWBoardDto[]> {
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
}
