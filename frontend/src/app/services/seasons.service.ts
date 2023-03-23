import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BackendService } from './backend.service';

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

@Injectable({
  providedIn: 'root',
})
export class SeasonsService {
  constructor(private backendService: BackendService) {}

  public getSeasons(): Observable<SeasonDto[]> {
    return this.backendService.get<SeasonDto[]>(URL);
  }

  public addSeason(season: SeasonDto): Observable<SeasonDto> {
    return this.backendService.post<SeasonDto, SeasonDto>(URL, season);
  }

  public deleteSeason(seasonId: string): Observable<SeasonDto> {
    return this.backendService.delete<SeasonDto>(URL, seasonId);
  }
}
