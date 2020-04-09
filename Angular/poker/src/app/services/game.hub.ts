import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr'
import { environment } from 'src/environments/environment';
import { Game } from '../models/dtos/game';

@Injectable({
  providedIn: 'root'
})
export class GameHub {
  public hubConnection: HubConnection;
  public game: Game;

  constructor() { }

  public startConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.hub}game?token=${sessionStorage['poker-game-jwt']}`)
      .build();

    this.hubConnection.start();
  }
}
