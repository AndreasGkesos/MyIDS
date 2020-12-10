import { Injectable, OnInit } from '@angular/core';
import { User, UserManager, WebStorageStateStore } from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  userManager: UserManager;
  
  constructor() {
    const settings = {
      userStore: new WebStorageStateStore({ store: window.localStorage }),
      authority: 'https://localhost:5001',
      client_id: 'angularclientCode',
      redirect_uri: 'https://localhost:4200/signin-callback',
      post_logout_redirect_uri: 'https://localhost:4200/signout-callback',
      response_type: 'code',
      scope: 'openid profile offline_access pokemonapi',
      response_mode: 'query'
    };
    this.userManager = new UserManager(settings);

  }

  public getUser(): Promise<User> {
    return this.userManager.getUser();
  }

  public login(): Promise<void> {
    return this.userManager.signinRedirect();
  }

  public signinCallback(): Promise<User> {
    return this.userManager.signinCallback();
  }

  public renewToken(): Promise<User> {
    return this.userManager.signinSilent();
  }

  public logout(): Promise<void> {
    return this.userManager.signoutRedirect();
  }

  public clear(): Promise<void> {
    return this.userManager.clearStaleState();
  }
}
