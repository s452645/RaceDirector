import { TestBed } from '@angular/core/testing';

import { RouteTitleService } from './route-title.service';

describe('RouteTitleService', () => {
  let service: RouteTitleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RouteTitleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
