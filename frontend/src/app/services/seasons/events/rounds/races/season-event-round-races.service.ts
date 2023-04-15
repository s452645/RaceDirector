import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';
import { SeasonEventRoundRaceDto } from '../season-event-rounds.service';

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

  public beginHeat(heatId: string): Observable<void> {
    return this.backendService.post(`${URL}/${heatId}/begin-heat`, null);
  }

  public saveDistanceAndBonuses(
    distance: number,
    bonuses: number[]
  ): Observable<void> {
    return this.backendService.post(`${URL}/save-heat-data`, {
      distance: distance,
      bonuses: bonuses,
    });
  }

  public endHeat(): Observable<void> {
    return this.backendService.post(`${URL}/end-heat`, null);
  }
}
