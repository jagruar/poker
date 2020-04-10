import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { GameListItem } from 'src/app/models/dtos/game-list-item';
import { environment } from 'src/environments/environment';
import { FormBuilder, Validators } from '@angular/forms';
import { JoinGameCommand } from 'src/app/models/commands/join-game.command';
import { HttpService } from 'src/app/services/http.service';
import { Router } from '@angular/router';
import { LobbyHub } from 'src/app/services/lobby.hub';

@Component({
  selector: 'app-game-list',
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.sass']
})
export class GameListComponent implements OnInit {
  public gameList: GameListItem[];
  public selectedGame: GameListItem;
  public joinForm;

  constructor(
    private http: HttpClient,
    private httpService: HttpService,
    private lobbyHub: LobbyHub,
    private formBuilder: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.joinForm = this.formBuilder.group({
      name: ['', Validators.required],
      buyIn: [100, Validators.required],
      password: ['', Validators.required],
    });

    this.http.get<GameListItem[]>(environment.api + 'games')
      .subscribe(x => this.gameList = x);

    this.lobbyHub.startConnection();
    this.lobbyHub.hubConnection.on('gamecreated', (game: GameListItem) => {
      this.gameList.push(game);
    })
  }

  submitJoin() {
    if (this.joinForm.valid){
      let joinGameCommand: JoinGameCommand = Object.assign({}, this.joinForm.value);
      this.httpService.postJoinGame(this.selectedGame.id, joinGameCommand)
        .subscribe(x => this.router.navigate(['play']));
    }
  }

  selectGame(game: GameListItem) {
    this.selectedGame = game;
  }

  createGame(){

  }
}
