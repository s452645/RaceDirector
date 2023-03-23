import { Injectable, OnDestroy } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { MOCK_BACKEND } from '../globals';
import { WebSocketMsg, WebSocketService } from './websocket.service';

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
  public syncMessages: Subject<WebSocketMsg> = new Subject();
  public syncBoardResponses: Subject<SyncBoardResponse> = new Subject();

  private subscription = new Subscription();

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private interval: any;

  constructor(private websocketService: WebSocketService) {
    if (MOCK_BACKEND) {
      this.mockWebSokcet();
      return;
    }

    this.syncMessages = this.websocketService.createSyncWebSocket(
      SYNC_WEB_SOCKET_ROUTE
    );

    const s = this.syncMessages.subscribe(message =>
      this.processSyncBoardResponse(message)
    );

    this.subscription.add(s);
  }

  ngOnDestroy(): void {
    if (this.interval) {
      clearInterval(this.interval);
    }

    this.subscription.unsubscribe();
  }

  private processSyncBoardResponse(webSocketMsg: WebSocketMsg): void {
    const response = this.tryParseSyncBoardResponse(webSocketMsg.content);

    if (!response || !('boardId' in response) || !('result' in response)) {
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

  private mockWebSokcet(): void {
    this.interval = setInterval(() => {
      const boardId = Math.round(Math.random() * 2);

      this.syncBoardResponses.next({
        boardId: `mock-board-${boardId}-id`,
        result: SyncBoardResult.SYNC_SUCCESS,
        currentSyncOffset: Math.round(Math.random() * 50) - 25,
        lastTenOffsetsAvg: undefined,
        newClockOffset: undefined,
        message: undefined,
      });
    }, 5000);
  }
}
