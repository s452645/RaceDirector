import { TestBed } from '@angular/core/testing';

import { PicoBoardsService } from './pico-boards.service';

describe('PicoBoardsService', () => {
  let service: PicoBoardsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PicoBoardsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
