import { Component, OnInit } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';
import { MakeMoveCommand } from 'src/app/models/commands/make-move.command';
import { Game } from 'src/app/models/dtos/game';
import { GameHub } from 'src/app/services/game.hub';
import { Player } from 'src/app/models/dtos/player';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.sass']
})
export class GameComponent implements OnInit {
  public game: Game;
  public player: Player;
  public isPlayersTurn: boolean;
  public players: Player[] = [];

  public canCheck: boolean; // if canCheck, its a bet
  public canCall: boolean; // if canCheck, its a bet
  public allInRequired: boolean; // if canCheck, its a bet
  public betMustBeAllIn: boolean; // if canCheck, its a bet

  public betForm;
  public raiseForm;

  constructor(
    public  gameHub: GameHub,
    private formBuilder: FormBuilder,
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

  public check() {
    let command: MakeMoveCommand = { amount: 0 };
    this.httpService.postMakeMoveCommand(command).toPromise();

  }

  public call() {
    let command: MakeMoveCommand = { amount: this.game.minimumMoveAmount };
    this.httpService.postMakeMoveCommand(command).toPromise();
  }

  public bet() {
    if (this.betForm.valid) {
      let command: MakeMoveCommand = Object.assign({}, this.betForm.value);
      this.httpService.postMakeMoveCommand(command).toPromise();
    }
  }

  public fold() {
    let command: MakeMoveCommand = { amount: null };
    this.httpService.postMakeMoveCommand(command).toPromise();
  }

  public allIn() {
    let command: MakeMoveCommand = { amount: this.player.balance };
    this.httpService.postMakeMoveCommand(command).toPromise();
  }

  public raise() {
    if (this.raiseForm.valid) {
      let command: MakeMoveCommand = Object.assign({}, this.raiseForm.value);
      this.httpService.postMakeMoveCommand(command).toPromise();
    }
  }

  public onGameStateChange(game: Game){
    console.log(game);    

    this.game = game;
    this.player = game.players.find(x => !!x.cards);
    this.isPlayersTurn = this.game.nextSeatNumber === this.player.seatNumber;    

    this.canCheck = game.minimumMoveAmount === 0;
    this.canCall =  game.minimumMoveAmount > 0 && game.minimumMoveAmount < this.player.balance;
    this.allInRequired = game.minimumMoveAmount === this.player.balance;
    this.betMustBeAllIn = game.minimumMoveAmount + game.minimumBetIncrement > this.player.balance;

    if (this.canCheck && !this.betMustBeAllIn) {
      this.betForm = this.formBuilder.group({
        amount: [this.game.minimumMoveAmount, Validators.required],
      });
    }

    if (this.canCall && !this.betMustBeAllIn) {
      this.raiseForm = this.formBuilder.group({
        amount: [this.game.minimumMoveAmount, Validators.required],
      });
    }

    if (!this.players) {
      for (let i = 0; i < game.maxPlayers; i++){
        this.players.push(null);
      }
    }

    game.players.forEach(x => this.players[x.seatNumber] = x);
  }
}
