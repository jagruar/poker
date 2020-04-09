import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GameListComponent } from './components/game-list/game-list.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { GameCreateComponent } from './components/game-create/game-create.component';
import { GameComponent } from './components/game/game.component';
import { TokenInterceptor } from './services/http-auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    GameListComponent,
    GameCreateComponent,
    GameComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
