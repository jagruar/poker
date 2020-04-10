import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr'
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LobbyHub {
  public hubConnection: HubConnection;

  constructor() { }

  public startConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.hub}lobby`)
      .build();

    this.hubConnection.start();
  }
}
