import { Component, OnInit } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';
import { MakeMoveCommand } from 'src/app/models/commands/make-move.command';
import { Game } from 'src/app/models/dtos/game';
import { GameHub } from 'src/app/services/game.hub';
import { Card } from 'src/app/models/dtos/card';
import { Player } from 'src/app/models/dtos/player';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.sass']
})
export class GameComponent implements OnInit {
  public game: Game;
  public player: Player;
  public isPlayersTurn: boolean;

  constructor(
    public  gameHub: GameHub,
    private httpService: HttpService) { }

  ngOnInit(): void {
    this.gameHub.startConnection();
    this.gameHub.hubConnection.on('gamestatechanged', (game: Game) => {
      this.onGameStateChange(game);
    });
  }

  public post() {
    // let command: MakeMoveCommand = { amount: 20}
    this.httpService.postStartGame().subscribe();
    //this.httpService.postMakeMoveCommand(command).subscribe();
  }

  public onGameStateChange(game: Game){
    this.game = game;
    this.player = game.players.find(x => !!x.cards);
    this.isPlayersTurn = this.game.nextSeatNumber === this.player.seatNumber;    
  }
}
