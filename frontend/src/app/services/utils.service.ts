import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UtilsService {
  public getNullableOrThrow<T>(nullable: T | null): T {
    if (nullable === null) {
      throw new Error('Nullable is null');
    }

    return nullable;
  }
}
