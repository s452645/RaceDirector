import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';

export enum RoundType {
  Ladder,
  Group,
  ClassicFinal,
  CandidateFinal,
}

export enum RoundPointsStrategy {
  OnlyThisRound,
  SummedWithPreviousRound,
}

export enum DroppedCarsPositionDefinementStrategy {
  RacePositionThenPoints,
  OnlyPoints,
}

export enum RaceOutcome {
  Undetermined,
  Advanced,
  Dropped,

  SecondChanceUndetermined,
  SecondChanceAdvanced,
  SecondChanceDropped,
}

const URL = `https://localhost:7219/api/SeasonEventRounds`;

export class CarDto {
  public id: string | undefined;

  constructor(public name: string) {}
}

export class SeasonEventRoundRaceHeatResultDto {
  public id: string | undefined;

  constructor(
    public carId: string,
    public car: CarDto,
    public sectorTimes: number[],
    public fullTime: number,
    public timePoints: number,
    public advantagePoints: number,
    public distancePoints: number,
    public bonuses: number[],
    public pointsSummed: number,
    public position: number
  ) {}
}

export class SeasonEventRoundRaceHeatDto {
  public id: string | undefined;

  constructor(
    public raceId: string,
    public results: SeasonEventRoundRaceHeatResultDto[]
  ) {}
}

export class SeasonEventRoundRaceResultDto {
  public id: string | undefined;

  constructor(
    public carId: string,
    public car: CarDto,
    public raceId: string,
    public position: number | null,
    public points: number,
    public raceOutcome: RaceOutcome
  ) {}
}

export class SeasonEventRoundRaceDto {
  public id: string | undefined;

  constructor(
    public order: number,
    public participantsCount: number,
    public results: SeasonEventRoundRaceResultDto[],
    public heats: SeasonEventRoundRaceHeatDto[],
    public instantAdvancements: number,
    public secondChances: number,
    public roundId?: string
  ) {}
}

export class SeasonEventRoundDto {
  public id: string | undefined;

  constructor(
    public order: number,
    public type: RoundType,
    public participantsCount: number,
    public participantsIds: string[],
    public races: SeasonEventRoundRaceDto[],
    public pointsStrategy: RoundPointsStrategy,
    public droppedCarsPositionDefinementStrategy: DroppedCarsPositionDefinementStrategy,
    public droppedCarsPointsStrategy: RoundPointsStrategy,
    public seasonEventId: string | undefined,
    public advancesCount: number | undefined
  ) {}
}

@Injectable({
  providedIn: 'root',
})
export class SeasonEventRoundsService {
  constructor(private backendService: BackendService) {}

  public getRounds(seasonEventId: string): Observable<SeasonEventRoundDto[]> {
    return this.backendService.get<SeasonEventRoundDto[]>(
      `${URL}?seasonEventId=${seasonEventId}`
    );
  }

  public getRound(
    seasonEventId: string,
    roundId: string
  ): Observable<SeasonEventRoundDto> {
    return this.backendService.get<SeasonEventRoundDto>(
      `${URL}/${roundId}?seasonEventId=${seasonEventId}`
    );
  }

  public addRound(
    seasonEventId: string,
    roundDto: SeasonEventRoundDto
  ): Observable<SeasonEventRoundDto> {
    return this.backendService.post<SeasonEventRoundDto, SeasonEventRoundDto>(
      `${URL}?seasonEventId=${seasonEventId}`,
      roundDto
    );
  }

  public drawRaces(roundId: string): Observable<SeasonEventRoundDto> {
    return this.backendService.post<unknown, SeasonEventRoundDto>(
      `${URL}/${roundId}/draw`,
      null
    );
  }

  public deleteRound(
    seasonEventId: string,
    roundId: string
  ): Observable<SeasonEventRoundDto> {
    return this.backendService.delete(
      `${URL}/${roundId}?seasonEventId=${seasonEventId}`
    );
  }
}
