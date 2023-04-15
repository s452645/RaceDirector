import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import {
  SyncBoardResponse,
  SyncDataService,
} from 'src/app/services/hardware/sync-data.service';

// class SyncedPicoWBoardDto implements DEPPicoWBoardDto {
//   public id: string;
//   public address: string;
//   public isConnected: boolean;
//   public lastOffset: number | undefined;

//   constructor(picoWBoard: DEPPicoWBoardDto) {
//     this.id = picoWBoard.id;
//     this.address = picoWBoard.address;
//     this.isConnected = picoWBoard.isConnected;
//   }
// }

@Component({
  selector: 'app-picos-sync-status',
  templateUrl: './picos-sync-status.component.html',
  styleUrls: ['./picos-sync-status.component.css'],
})
export class PicosSyncStatusComponent implements OnInit, OnDestroy {
  boardsOffsetDict = new Map<string, string>();

  private subscription = new Subscription();

  constructor(private syncDataService: SyncDataService) {}

  ngOnInit(): void {
    this.syncDataService.createSyncSocket();

    this.subscription.add(
      this.syncDataService.syncBoardResponses.subscribe(response =>
        this.processNewSyncResponse(response)
      )
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  getNgClassForOffset(value: string): string {
    const offset = parseInt(value);

    if (Math.abs(offset) < 10) {
      return 'text-green-600';
    }

    if (Math.abs(offset) < 25) {
      return 'text-yellow-600';
    }

    return 'text-pink-600';
  }

  private processNewSyncResponse(response: any): void {
    this.boardsOffsetDict.set(
      response.PicoBoardId,
      response.CurrentSyncOffset?.toString() ?? '??'
    );
  }
}
