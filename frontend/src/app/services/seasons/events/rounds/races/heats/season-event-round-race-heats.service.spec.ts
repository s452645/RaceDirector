import { TestBed } from '@angular/core/testing';

import { SeasonEventRoundRaceHeatsService } from './season-event-round-race-heats.service';

describe('SeasonEventRoundRaceHeatsService', () => {
  let service: SeasonEventRoundRaceHeatsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SeasonEventRoundRaceHeatsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
