import { Component, OnInit } from '@angular/core';
import { DEPPicoWBoardDto } from 'src/app/services/hardware/pico-boards.service';
import { BaseContainerComponent } from '../base-container/base-container.component';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
})
export class SettingsComponent
  extends BaseContainerComponent
  implements OnInit
{
  public boardId = '';
  public ipAddress = '';

  boards: DEPPicoWBoardDto[] = [];

  override ngOnInit(): void {
    super.ngOnInit();

    this.picoBoardsService
      .DEPgetBoards()
      .subscribe(boards => (this.boards = boards));
  }

  public handleClick(): void {
    this.picoBoardsService
      .DEPaddBoard(this.boardId, this.ipAddress)
      .subscribe(resp => console.log(JSON.stringify(resp)));

    this.boardId = '';
    this.ipAddress = '';
  }

  public refreshBoards(): void {
    this.picoBoardsService
      .DEPgetBoards()
      .subscribe(boards => (this.boards = boards));
  }

  // TODO: sometimes, there is a need to refresh the page before creating the socket
  // .NET doesn't even receive a request then (investigate)
  public createSyncWebSocket() {
    // this.webSocketService.createSyncWebSocket();
    // const s = this.webSocketService.messages.subscribe(msg => {
    //   console.log('Response from websocket: ' + msg);
    // });
    // this.subscription.add(s);
  }
}
