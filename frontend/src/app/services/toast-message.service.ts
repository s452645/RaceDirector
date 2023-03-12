import { Injectable } from '@angular/core';
import { Message } from 'primeng/api';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ToastMessageService {
  public messageSubject = new Subject<Message>();

  public createToastMessage(message: Message): void {
    this.messageSubject.next(message);
  }
}
