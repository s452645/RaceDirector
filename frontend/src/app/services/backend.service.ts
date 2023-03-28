import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastMessageService } from './toast-message.service';

@Injectable({
  providedIn: 'root',
})
export class BackendService {
  constructor(
    private httpClient: HttpClient,
    private toastMessageService: ToastMessageService
  ) {}

  public get<T>(url: string): Observable<T> {
    return this.httpClient
      .get<T>(url)
      .pipe(catchError(this.handleError.bind(this)));
  }

  public post<T, U>(url: string, body: T): Observable<U> {
    return this.httpClient
      .post<U>(url, body)
      .pipe(catchError(this.handleError.bind(this)));
  }

  public delete<T>(url: string): Observable<T> {
    return this.httpClient
      .delete<T>(url)
      .pipe(catchError(this.handleError.bind(this)));
  }

  public textRequest(message: string): Observable<string> {
    return this.httpClient
      .post<string>(
        `https://localhost:7219/api/Cars/msg?message=${message}`,
        {}
      )
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      const msg = `An error occurred: ${error.error}`;

      console.error(msg);
      this.toastMessageService.createToastMessage({
        severity: 'error',
        summary: 'An error occurred',
        detail: JSON.stringify(error.message),
      });
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      const msg = `Backend returned code ${error.status}, body was: ${error.error}`;

      console.error(msg);
      this.toastMessageService.createToastMessage({
        severity: 'error',
        summary: 'An error occurred',
        detail: `[${error.status}]: ${JSON.stringify(error.message)}`,
      });
    }
    // Return an observable with a user-facing error message.
    return throwError(
      () => new Error('Something bad happened; please try again later.')
    );
  }
}
