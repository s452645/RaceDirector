import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BackendService } from 'src/app/services/backend.service';
import { CarDto } from 'src/app/services/cars/car.service';

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

export enum RaceState {
  Inactive,
  Pending,
  Active,
}

const URL = `https://localhost:7219/api/SeasonEventRounds`;

export class SeasonEventRoundRaceResultData {
  constructor(
    public raceResultId: string,
    public distance: number,
    public bonusesPoints: number
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
    public raceOutcome: RaceOutcome,
    public sectorTimes: number[],
    public distance: number,
    public bonusesPoints: number,
    public raceState: RaceState
  ) {}

  public static fromPayload(payload: any): SeasonEventRoundRaceResultDto {
    const resultDto = new SeasonEventRoundRaceResultDto(
      payload?.carId,
      CarDto.fromPayload(payload?.car),
      payload?.raceId,
      payload?.position,
      payload?.points,
      payload?.raceOutcome,
      payload?.sectorTimes ?? [],
      payload?.distance,
      payload?.bonusesPoints,
      payload?.raceState
    );

    resultDto.id = payload?.id;
    return resultDto;
  }

  public static fromWebScocketPayload(
    payload: any
  ): SeasonEventRoundRaceResultDto {
    const resultDto = new SeasonEventRoundRaceResultDto(
      payload?.CarId,
      CarDto.fromWebsocketPayload(payload?.Car),
      payload?.RaceId,
      payload?.Position,
      payload?.Points,
      payload?.RaceOutcome,
      payload?.SectorTimes ?? [],
      payload?.Distance,
      payload?.BonusesPoints,
      payload?.RaceState
    );

    resultDto.id = payload?.Id;
    return resultDto;
  }
}

export class SeasonEventRoundRaceDto {
  public id: string | undefined;

  constructor(
    public order: number,
    public participantsCount: number,
    public results: SeasonEventRoundRaceResultDto[],
    public instantAdvancements: number,
    public secondChances: number,
    public roundId?: string
  ) {}

  public static fromPayload(payload: any): SeasonEventRoundRaceDto {
    const raceDto = new SeasonEventRoundRaceDto(
      payload?.order,
      payload.participantsCount,
      payload?.results?.map((r: any) =>
        SeasonEventRoundRaceResultDto.fromPayload(r)
      ) ?? [],
      payload?.instantAdvancements,
      payload?.secondChances,
      payload?.roundId
    );

    raceDto.id = payload?.id;
    return raceDto;
  }

  public static fromWebSocketPayload(payload: any): SeasonEventRoundRaceDto {
    const raceDto = new SeasonEventRoundRaceDto(
      payload?.Order,
      payload.ParticipantsCount,
      payload?.Results?.map((r: any) =>
        SeasonEventRoundRaceResultDto.fromWebScocketPayload(r)
      ) ?? [],
      payload?.InstantAdvancements,
      payload?.SecondChances,
      payload?.RoundId
    );

    raceDto.id = payload?.Id;
    return raceDto;
  }
}

export class SeasonEventRoundDto {
  public id: string | undefined;

  constructor(
    public order: number,
    public type: RoundType,
    public participantsCount: number,
    public participantsIds: string[],
    public participantsNames: string[],
    public races: SeasonEventRoundRaceDto[],
    public pointsStrategy: RoundPointsStrategy,
    public droppedCarsPositionDefinementStrategy: DroppedCarsPositionDefinementStrategy,
    public droppedCarsPointsStrategy: RoundPointsStrategy,
    public seasonEventId: string | undefined,
    public advancesCount: number | undefined
  ) {}

  public static fromPayload(payload: any): SeasonEventRoundDto {
    const round = new SeasonEventRoundDto(
      payload?.order,
      payload?.type,
      payload?.participantsCount,
      payload?.participantsIds ?? [],
      payload?.participantsNames ?? [],
      payload?.races?.map((r: any) => SeasonEventRoundRaceDto.fromPayload(r)) ??
        [],
      payload?.pointsStrategy,
      payload?.droppedCarsPositionDefinementStrategy,
      payload?.droppedCarsPointsStrategy,
      payload?.seasonEventId,
      payload?.advancesCount
    );

    round.id = payload?.id;
    return round;
  }

  get typeText(): string {
    switch (this.type) {
      case RoundType.Ladder:
        return 'Ladder';
      case RoundType.Group:
        return 'Group';
      case RoundType.ClassicFinal:
        return 'Classic Final';
      case RoundType.CandidateFinal:
        return 'Candidate Final';
      default:
        return 'Unknown';
    }
  }
}

@Injectable({
  providedIn: 'root',
})
export class SeasonEventRoundsService {
  constructor(private backendService: BackendService) {}

  public getRounds(seasonEventId: string): Observable<SeasonEventRoundDto[]> {
    return this.backendService
      .get<SeasonEventRoundDto[]>(`${URL}?seasonEventId=${seasonEventId}`)
      .pipe(
        map(rounds =>
          rounds.map(round => SeasonEventRoundDto.fromPayload(round))
        )
      );
  }

  public getRound(
    seasonEventId: string,
    roundId: string
  ): Observable<SeasonEventRoundDto> {
    return this.backendService
      .get<SeasonEventRoundDto>(
        `${URL}/${roundId}?seasonEventId=${seasonEventId}`
      )
      .pipe(map(round => SeasonEventRoundDto.fromPayload(round)));
  }

  public addRound(
    seasonEventId: string,
    roundDto: SeasonEventRoundDto
  ): Observable<SeasonEventRoundDto> {
    return this.backendService
      .post<SeasonEventRoundDto, SeasonEventRoundDto>(
        `${URL}?seasonEventId=${seasonEventId}`,
        roundDto
      )
      .pipe(map(round => SeasonEventRoundDto.fromPayload(round)));
  }

  public updateRound(
    seasonEventId: string,
    roundDto: SeasonEventRoundDto
  ): Observable<SeasonEventRoundDto> {
    return this.backendService
      .put(`${URL}?seasonEventId=${seasonEventId}`, roundDto)
      .pipe(map(round => SeasonEventRoundDto.fromPayload(round)));
  }

  public drawRaces(roundId: string): Observable<SeasonEventRoundDto> {
    return this.backendService
      .post<unknown, SeasonEventRoundDto>(`${URL}/${roundId}/draw`, null)
      .pipe(map(round => SeasonEventRoundDto.fromPayload(round)));
  }

  public deleteRound(seasonEventId: string, roundId: string): Observable<void> {
    return this.backendService.delete(
      `${URL}/${roundId}?seasonEventId=${seasonEventId}`
    );
  }

  public hasRoundStarted(roundId: string): Observable<boolean> {
    return this.backendService.get(`${URL}/${roundId}/hasStarted`);
  }
}
