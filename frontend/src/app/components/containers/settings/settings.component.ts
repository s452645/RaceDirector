import { Component, OnInit } from '@angular/core';
import {
  PicoBoardDto,
  PicoBoardType,
} from 'src/app/services/hardware/pico-boards.service';
import { BaseContainerComponent } from '../base-container/base-container.component';
import { AbstractControl, FormGroup, Validators } from '@angular/forms';
import { SelectItem } from 'primeng/api';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
})
export class SettingsComponent
  extends BaseContainerComponent
  implements OnInit
{
  boardTypeOptions: SelectItem[] = [
    {
      label: 'USB',
      value: PicoBoardType.USB,
    },
    {
      label: 'WiFi',
      value: PicoBoardType.WiFi,
    },
  ];

  newBoardForm: FormGroup = new FormGroup({});

  boards: PicoBoardDto[] = [];
  selectedBoardId: string | undefined = undefined;
  isSensorsDialogOpen = false;

  override ngOnInit(): void {
    super.ngOnInit();

    this.refreshBoards();
    this.refreshForm();
  }

  public refreshBoards(): void {
    this.picoBoardsService
      .getBoards()
      .subscribe(boards => (this.boards = boards));
  }

  public handleOpenSensorsDialog(boardId: string): void {
    this.selectedBoardId = boardId;
    this.isSensorsDialogOpen = true;
  }

  public handleDeleteBoard(boardId: string): void {
    this.subscription.add(
      this.picoBoardsService
        .deleteBoard(boardId)
        .subscribe(() => this.refreshBoards())
    );
  }

  public getSelectedBoardName(): string | null {
    if (!this.selectedBoardId) {
      return null;
    }

    const selectedBoard = this.boards.find(b => b.id === this.selectedBoardId);

    if (!selectedBoard) {
      return null;
    }

    return selectedBoard.name;
  }

  public handleSubmitNewBoard(): void {
    const type = this.utils.getUndefinedableOrThrow(
      this.newBoardForm.get('type')?.value
    ).value;

    const name = this.utils.getUndefinedableOrThrow(
      this.newBoardForm.get('name')?.value
    );

    const ipAddress = this.utils.getUndefinedableOrThrow(
      this.newBoardForm.get('ipAddress')?.value
    );

    const boardDto = new PicoBoardDto(
      type,
      name,
      ipAddress,
      false,
      false,
      [],
      undefined,
      undefined,
      undefined
    );

    this.subscription.add(
      this.picoBoardsService.addBoard(boardDto).subscribe(() => {
        this.refreshBoards();
        this.refreshForm();
      })
    );
  }

  public syncAll(): void {
    this.subscription.add(
      this.picoBoardsService.syncAll().subscribe(() => {
        this.refreshBoards();
        this.refreshForm();
      })
    );
  }

  public listenToEventsAll(): void {
    this.subscription.add(
      this.picoBoardsService.listenToEventsAll().subscribe(() => {
        return;
      })
    );
  }

  private refreshForm(): void {
    this.newBoardForm = this.fb.group({
      type: [null as SelectItem | null, Validators.required],
      name: ['', Validators.required],
      ipAddress: [
        '',
        (control: AbstractControl) => {
          if (
            PicoBoardType.WiFi ===
              this.newBoardForm.get('type')?.value?.value &&
            !control.value
          ) {
            return { missingIpAddress: true };
          }

          return null;
        },
      ],
    });

    this.subscription.add(
      this.newBoardForm.get('type')?.valueChanges.subscribe(value => {
        if (value.value === PicoBoardType.USB) {
          const ipAddressInput = this.newBoardForm.get('ipAddress');
          ipAddressInput?.disable();
          ipAddressInput?.setValue(null);
        } else {
          this.newBoardForm.get('ipAddress')?.enable();
        }
      })
    );
  }
}
