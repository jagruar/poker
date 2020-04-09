import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { JoinGameCommand } from '../models/commands/join-game.command';
import { environment } from 'src/environments/environment';
import { GameToke as GameToken } from '../models/dtos/game-token';
import { CreateGameCommand } from '../models/commands/create-game.command';
import { MakeMoveCommand } from '../models/commands/make-move.command';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private http: HttpClient) { }

  postCreateGame(command: CreateGameCommand): Observable<GameToken> {
    return this.http.post<GameToken>(`${environment.api}games`, command)
    .pipe(
      tap(x => {console.log(x); sessionStorage['poker-game-jwt'] = x.token}),
      catchError(this.handleError)
    );
  }

  postJoinGame(gameId: string, command: JoinGameCommand): Observable<GameToken> {
    return this.http.post<GameToken>(`${environment.api}games/${gameId}/players`, command)
    .pipe(
      tap(x => sessionStorage['poker-game-jwt'] = x.token),
      catchError(this.handleError)
    );
  }

  postStartGame() {
    return this.http.post(`${environment.api}games/start`, null, { headers: this.getGameTokenHeaders() })
    .pipe(
      catchError(this.handleError)
    );
  }

  postMakeMoveCommand(command: MakeMoveCommand) {
    return this.http.post(`${environment.api}games/play`, command, { headers: this.getGameTokenHeaders() })
    .pipe(
      catchError(this.handleError)
    );
  }

  private getGameTokenHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    headers.append('Authorization', 'Bearer ' + sessionStorage['poker-game-jwt']);
    return headers;
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  };
}
