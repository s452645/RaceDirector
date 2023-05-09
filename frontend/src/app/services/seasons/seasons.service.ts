import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
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

  public static fromPayload(payload: any): SeasonDto {
    const seasonDto = new SeasonDto(
      payload?.name,
      new Date(payload?.startDate),
      new Date(payload?.endDate)
    );

    seasonDto.id = payload?.id;
    return seasonDto;
  }

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

  public static fromPayload(payload: any): SeasonEventScoreRulesDto {
    const rules = new SeasonEventScoreRulesDto(
      payload?.timeMultiplier,
      payload?.distanceMultiplier,
      payload?.availableBonuses ?? [],
      payload?.unfinishedSectorPenaltyPoints,
      payload?.theMoreTheBetter,
      payload?.seasonEventId
    );

    rules.id = payload?.id;
    return rules;
  }
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

  public static fromPayload(payload: any): SeasonEventDto {
    const newEvent = new SeasonEventDto(
      payload?.name,
      new Date(payload?.startDate),
      new Date(payload?.endDate),
      payload?.type,
      SeasonEventScoreRulesDto.fromPayload(payload?.scoreRules),
      CircuitDto.fromPayload(payload?.circuit),
      payload?.participantsCount
    );

    newEvent.id = payload?.id;
    return newEvent;
  }

  get typeText(): string {
    switch (this.type) {
      case SeasonEventType.Race:
        return 'Race';
      case SeasonEventType.TimeTrial:
        return 'Time Trial';
      default:
        return 'Unknown';
    }
  }
}

@Injectable({
  providedIn: 'root',
})
export class SeasonsService {
  constructor(private backendService: BackendService) {}

  public getSeasons(): Observable<SeasonDto[]> {
    return this.backendService
      .get<SeasonDto[]>(URL)
      .pipe(map(seasons => seasons.map(s => SeasonDto.fromPayload(s))));
  }

  public getSeasonById(id: string): Observable<SeasonDto> {
    return this.backendService
      .get<SeasonDto>(`${URL}/${id}`)
      .pipe(map(s => SeasonDto.fromPayload(s)));
  }

  public addSeason(season: SeasonDto): Observable<SeasonDto> {
    return this.backendService
      .post<SeasonDto, SeasonDto>(URL, season)
      .pipe(map(s => SeasonDto.fromPayload(s)));
  }

  public deleteSeason(seasonId: string): Observable<SeasonDto> {
    return this.backendService
      .delete<SeasonDto>(`${URL}/${seasonId}`)
      .pipe(map(s => SeasonDto.fromPayload(s)));
  }

  public getSeasonEvents(seasonId: string): Observable<SeasonEventDto[]> {
    return this.backendService
      .get<SeasonEventDto[]>(`${URL}/${seasonId}/season-events`)
      .pipe(
        map(events => events.map(event => SeasonEventDto.fromPayload(event)))
      );
  }

  public getSeasonEventById(
    seasonId: string,
    seasonEventId: string
  ): Observable<SeasonEventDto> {
    return this.backendService
      .get<SeasonEventDto>(`${URL}/${seasonId}/season-events/${seasonEventId}`)
      .pipe(map(event => SeasonEventDto.fromPayload(event)));
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

  public updateSeasonEvent(
    seasonId: string,
    seasonEvent: SeasonEventDto
  ): Observable<SeasonEventDto> {
    return this.backendService
      .put<SeasonEventDto, SeasonEventDto>(
        `${URL}/${seasonId}/season-events`,
        seasonEvent
      )
      .pipe(map(event => SeasonEventDto.fromPayload(event)));
  }

  public deleteSeasonEvent(
    seasonId: string,
    seasonEventId: string
  ): Observable<SeasonEventDto> {
    return this.backendService
      .delete<SeasonEventDto>(
        `${URL}/${seasonId}/season-events/${seasonEventId}`
      )
      .pipe(map(event => SeasonEventDto.fromPayload(event)));
  }

  public addSeasonEventScoreRules(
    seasonEventId: string,
    scoreRules: SeasonEventScoreRulesDto
  ): Observable<SeasonEventDto> {
    return this.backendService
      .post<SeasonEventScoreRulesDto, SeasonEventDto>(
        `${URL}/${seasonEventId}/score-rules`,
        scoreRules
      )
      .pipe(map(event => SeasonEventDto.fromPayload(event)));
  }
}
