import { Injectable } from '@angular/core';
import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { ToastMessageService } from './toast-message.service';
import { Message } from 'primeng/api';

export interface WebSocketMsg {
  source: string;
  content: string;
}

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  constructor(private toastMessageService: ToastMessageService) {}

  public createSyncWebSocket<T>(route: string): Subject<T> {
    return <Subject<T>>this.connect(`wss://localhost:7219/${route}`).pipe(
      map((response: MessageEvent): T => {
        return response.data;
      })
    );
  }

  private connect(url: string): AnonymousSubject<MessageEvent> {
    const subject = this.create(url);
    console.log(`Socket successfully connected on ${url}`);
    return subject;
  }

  private create(url: string): AnonymousSubject<MessageEvent> {
    const ws = new WebSocket(url);
    const observable = new Observable((obs: Observer<MessageEvent>) => {
      ws.onmessage = obs.next.bind(obs);
      ws.onerror = obs.error.bind(obs);
      ws.onclose = obs.complete.bind(obs);
      return ws.close.bind(ws);
    });

    const observer = {
      complete: () => {
        return;
      },
      error: (err: Error) => {
        console.error(JSON.stringify(err));
        this.createToastMessage(err.message);
      },
      next: (data: unknown) => {
        console.log('Message sent to websocket: ', data);
        if (ws.readyState === WebSocket.OPEN) {
          ws.send(JSON.stringify(data));
        }
      },
    };

    return new AnonymousSubject<MessageEvent>(observer, observable);
  }

  private createToastMessage(message: string): void {
    const toastMsg: Message = {
      severity: 'error',
      summary: 'Web socket error',
      detail: message,
    };

    this.toastMessageService.createToastMessage(toastMsg);
  }
}
