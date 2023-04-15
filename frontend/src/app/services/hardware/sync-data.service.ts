import { Injectable, OnDestroy } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { WebSocketMsg, WebSocketService } from '../websocket.service';

const SYNC_WEB_SOCKET_ROUTE = 'sync';

export enum SyncBoardResult {
  SYNC_SUCCESS,
  SYNC_DROPPED,
  SYNC_SUSPICIOUS,
  SYNC_ERROR,
}

export interface SyncBoardResponse {
  boardId: string;
  result: SyncBoardResult;
  currentSyncOffset: number | undefined;
  lastTenOffsetsAvg: number | undefined;
  newClockOffset: number | undefined;
  message: string | undefined;
}

@Injectable({
  providedIn: 'root',
})
export class SyncDataService implements OnDestroy {
  public syncMessages: Subject<string> = new Subject();
  public syncBoardResponses: Subject<SyncBoardResponse> = new Subject();

  private subscription = new Subscription();

  constructor(private websocketService: WebSocketService) {}

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public createSyncSocket(): void {
    this.syncMessages = this.websocketService.createSyncWebSocket(
      SYNC_WEB_SOCKET_ROUTE
    );

    this.subscription.add(
      this.syncMessages.subscribe(message =>
        this.processSyncBoardResponse(message)
      )
    );
  }

  private processSyncBoardResponse(webSocketMsg: string): void {
    const response = this.tryParseSyncBoardResponse(webSocketMsg);

    if (
      !response ||
      !('SyncResult' in response) ||
      !('PicoBoardId' in response)
    ) {
      console.error('Processing sync web socket message failed');
      return;
    }

    this.syncBoardResponses.next(response);
  }

  private tryParseSyncBoardResponse(
    data: string
  ): SyncBoardResponse | undefined {
    try {
      return JSON.parse(data);
    } catch (err) {
      console.error(
        `Error while parsing web socket message: ${JSON.stringify(err)}`
      );
      return;
    }
  }
}
