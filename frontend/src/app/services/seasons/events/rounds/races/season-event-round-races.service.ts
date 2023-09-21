import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';
import {
  SeasonEventRoundRaceDto,
  SeasonEventRoundRaceResultData,
} from '../season-event-rounds.service';

const URL = `https://localhost:7219/api/SeasonEventRoundRaces`;

@Injectable({
  providedIn: 'root',
})
export class SeasonEventRoundRacesService {
  constructor(private backendService: BackendService) {}

  public getRace(
    roundId: string,
    raceId: string
  ): Observable<SeasonEventRoundRaceDto> {
    return this.backendService.get(`${URL}/${raceId}?roundId=${roundId}`);
  }

  public getRaceAvailableBonuses(raceId: string): Observable<number[]> {
    return this.backendService.get(`${URL}/${raceId}/bonuses`);
  }

  public saveDistanceAndBonuses(
    raceId: string,
    raceData: SeasonEventRoundRaceResultData
  ): Observable<void> {
    return this.backendService.post(`${URL}/${raceId}/race-data`, raceData);
  }

  public startRace(raceId: string, raceResultId: string): Observable<void> {
    return this.backendService.post(
      `${URL}/${raceId}/${raceResultId}/start`,
      null
    );
  }

  public DNFrace(raceId: string, raceResultId: string): Observable<void> {
    return this.backendService.post(
      `${URL}/${raceId}/${raceResultId}/dnf`,
      null
    );
  }
}
