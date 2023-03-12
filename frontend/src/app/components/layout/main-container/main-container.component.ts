import { Component, OnDestroy } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ToastMessageService } from 'src/app/services/toast-message.service';

@Component({
  selector: 'app-main-container',
  templateUrl: './main-container.component.html',
  styleUrls: ['./main-container.component.css'],
  providers: [MessageService],
})
export class MainContainerComponent implements OnDestroy {
  private subscription = new Subscription();

  constructor(
    private messageService: MessageService,
    private toastMessageService: ToastMessageService
  ) {
    const s = this.toastMessageService.messageSubject.subscribe(msg => {
      this.messageService.add(msg);
    });

    this.subscription.add(s);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
