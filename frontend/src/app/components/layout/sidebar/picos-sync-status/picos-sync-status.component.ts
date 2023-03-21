import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import {
  PicoBoardsService,
  PicoWBoardDto,
} from 'src/app/services/pico-boards.service';
import {
  SyncBoardResponse,
  SyncDataService,
} from 'src/app/services/sync-data.service';

class SyncedPicoWBoardDto implements PicoWBoardDto {
  public id: string;
  public address: string;
  public isConnected: boolean;
  public lastOffset: number | undefined;

  constructor(picoWBoard: PicoWBoardDto) {
    this.id = picoWBoard.id;
    this.address = picoWBoard.address;
    this.isConnected = picoWBoard.isConnected;
  }
}

@Component({
  selector: 'app-picos-sync-status',
  templateUrl: './picos-sync-status.component.html',
  styleUrls: ['./picos-sync-status.component.css'],
})
export class PicosSyncStatusComponent implements OnInit, OnDestroy {
  public boards: SyncedPicoWBoardDto[] = [];

  private subscription = new Subscription();

  constructor(
    private picoBoardsService: PicoBoardsService,
    private syncDataService: SyncDataService
  ) {}

  ngOnInit(): void {
    this.subscription.add(
      this.picoBoardsService
        .getBoards()
        .subscribe(boards =>
          boards.forEach(board =>
            this.boards.push(new SyncedPicoWBoardDto(board))
          )
        )
    );

    this.subscription.add(
      this.syncDataService.syncBoardResponses.subscribe(response =>
        this.processNewSyncResponse(response)
      )
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private processNewSyncResponse(response: SyncBoardResponse): void {
    const board = this.boards.find(board => board.id === response.boardId);
    if (!board) {
      console.error(`Unrecognized board: ${response.boardId}`);
      return;
    }

    board.lastOffset = response.currentSyncOffset;
  }
}
