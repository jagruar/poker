import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GameListComponent } from './components/game-list/game-list.component';
import { GameCreateComponent } from './components/game-create/game-create.component';
import { GameComponent } from './components/game/game.component';


const routes: Routes = [
  { path: '', component: GameListComponent },
  { path: 'start', component: GameCreateComponent },
  { path: 'play', component: GameComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
