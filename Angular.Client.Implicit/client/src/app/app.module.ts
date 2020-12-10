import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { SigninCallbackComponent } from './components/signin-callback/signin-callback.component';
import { AppRoutingModule } from './app-routing.module';
import { SignoutCallbackComponent } from './components/signout-callback/signout-callback.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './core/token.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    SigninCallbackComponent,
    SignoutCallbackComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
  }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
