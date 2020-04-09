import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpService } from 'src/app/services/http.service';
import { Router } from '@angular/router';
import { CreateGameCommand } from 'src/app/models/commands/create-game.command';

@Component({
  selector: 'app-game-create',
  templateUrl: './game-create.component.html',
  styleUrls: ['./game-create.component.sass']
})
export class GameCreateComponent implements OnInit {
  public form;

  constructor(
    private formBuilder: FormBuilder,
    private httpService: HttpService,
    private router: Router) { }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      password: ['', Validators.required],
      maxPlayers: [8, Validators.required],
      adminName: ['', Validators.required],
      adminBuyIn: [100, Validators.required],
      smallBlind: [1, Validators.required],
      bigBlind: [2, Validators.required],
    })
  }

  createGame(){
    if (this.form.valid) {
      let command: CreateGameCommand = Object.assign({}, this.form.value);
      this.httpService.postCreateGame(command)
        .subscribe(x => this.router.navigate(['play']));
      
    }

  }

}
