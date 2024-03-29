import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from '../backend.service';
import { CircuitDto } from './events/circuit.service';

const URL = 'https://localhost:7219/api/Seasons';

export class SeasonDto {
  id: string | undefined;

  constructor(
    public name: string,
    public startDate: Date,
    public endDate: Date
  ) {}

  // TODO: standings
}

export class SeasonEventScoreRulesDto {
  id: string | undefined;

  constructor(
    public timeMultiplier: number,
    public distanceMultiplier: number,
    public availableBonuses: number[],
    public unfinishedSectorPenaltyPoints: number,
    public theMoreTheBetter: boolean,
    public seasonEventId: string
  ) {}
}

export enum SeasonEventType {
  Race,
  TimeTrial,
}

export class SeasonEventDto {
  id: string | undefined;

  constructor(
    public name: string,
    public startDate: Date | undefined,
    public endDate: Date | undefined,
    public type: SeasonEventType,
    public scoreRules: SeasonEventScoreRulesDto | undefined,
    public circuit: CircuitDto | undefined,
    public participantsCount: number | undefined
  ) {}
}

@Injectable({
  providedIn: 'root',
})
export class SeasonsService {
  constructor(private backendService: BackendService) {}

  public getSeasons(): Observable<SeasonDto[]> {
    return this.backendService.get<SeasonDto[]>(URL);
  }

  public getSeasonById(id: string): Observable<SeasonDto> {
    return this.backendService.get<SeasonDto>(`${URL}/${id}`);
  }

  public addSeason(season: SeasonDto): Observable<SeasonDto> {
    return this.backendService.post<SeasonDto, SeasonDto>(URL, season);
  }

  public deleteSeason(seasonId: string): Observable<SeasonDto> {
    return this.backendService.delete<SeasonDto>(`${URL}/${seasonId}`);
  }

  public getSeasonEvents(seasonId: string): Observable<SeasonEventDto[]> {
    return this.backendService.get<SeasonEventDto[]>(
      `${URL}/${seasonId}/season-events`
    );
  }

  public getSeasonEventById(
    seasonId: string,
    seasonEventId: string
  ): Observable<SeasonEventDto> {
    return this.backendService.get<SeasonEventDto>(
      `${URL}/${seasonId}/season-events/${seasonEventId}`
    );
  }

  public addSeasonEvent(
    seasonId: string,
    seasonEvent: SeasonEventDto
  ): Observable<void> {
    return this.backendService.post<SeasonEventDto, void>(
      `${URL}/${seasonId}/season-events`,
      seasonEvent
    );
  }

  public deleteSeasonEvent(
    seasonId: string,
    seasonEventId: string
  ): Observable<SeasonEventDto> {
    return this.backendService.delete<SeasonEventDto>(
      `${URL}/${seasonId}/season-events/${seasonEventId}`
    );
  }

  public addSeasonEventScoreRules(
    seasonEventId: string,
    scoreRules: SeasonEventScoreRulesDto
  ): Observable<SeasonEventDto> {
    return this.backendService.post<SeasonEventScoreRulesDto, SeasonEventDto>(
      `${URL}/${seasonEventId}/score-rules`,
      scoreRules
    );
  }
}
